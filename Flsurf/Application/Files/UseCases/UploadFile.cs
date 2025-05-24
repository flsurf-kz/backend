using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Domain.Files.Entities;
using Flsurf.Infrastructure.Adapters.FileStorage;
using Hangfire.Logging;
using Microsoft.EntityFrameworkCore;
using MimeDetective;
using MimeDetective.Definitions;
using System.Net;
using System.Security;

namespace Flsurf.Application.Files.UseCases
{
    /// <summary>Юз-кейс загрузки файла (из URL или из запроса).</sumPosition mary>
    public class UploadFile : BaseUseCase<CreateFileDto, FileEntity>
    {
        private const int MaxRemoteBytes = 20 * 1024 * 1024;

        // «белые» хосты для скачивания (Prod-режим)
        private static readonly string[] AllowedHosts =
        {
            "cdn.example.com",
            "storage.googleapis.com"
        };

        private readonly IApplicationDbContext _db;
        private readonly IFileStorageAdapter _storage;
        private readonly HttpClient _http;
        private readonly ILogger<UploadFile> _log;
        private readonly IHostEnvironment _env;

        public UploadFile(
            IApplicationDbContext db,
            IFileStorageAdapter storage,
            IHttpClientFactory httpFactory,
            ILogger<UploadFile> logger,
            IHostEnvironment env)
        {
            _db = db;
            _storage = storage;
            _http = httpFactory.CreateClient();
            _log = logger;
            _env = env;
        }

        public async Task<FileEntity> Execute(CreateFileDto dto)
        {
            /*─ 1. уже есть FileId ─────────────────────────────────────────*/
            if (dto.FileId is { } id)
            {
                _log.LogInformation("Returning existing file {FileId}", id);
                var ent = await _db.Files.FirstOrDefaultAsync(x => x.Id == id);
                Guard.Against.Null(ent, message: $"Image does not exist: {id}");
                return ent!;
            }

            /*─ 2. получаем поток ─────────────────────────────────────────*/
            await using var baseStream = await GetStreamAsync(dto);
            Stream stream = baseStream;

            /*─ 3. сверяем MIME (если задан) ──────────────────────────────*/
            if (!string.IsNullOrWhiteSpace(dto.MimeType))
            {
                if (!stream.CanSeek)
                {
                    // делаем seek-able: копируем во временный MemoryStream
                    stream = await ToMemoryStreamAsync(stream);
                }

                var realMime = await DetectMimeAsync(stream);
                if (!realMime.Equals(dto.MimeType, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Declared MIME '{dto.MimeType}' " +
                                                        $"does not match real '{realMime}'.");

                // сбрасываем позицию, только если можем
                if (stream.CanSeek) stream.Position = 0;
            }

            /*─ 4. сохраняем в хранилище ─────────────────────────────────*/
            var originalName = string.IsNullOrWhiteSpace(dto.Name)
                             ? Guid.NewGuid().ToString()
                             : dto.Name!;

            var relativePath = $"files/{Guid.NewGuid():N}{Path.GetExtension(originalName)}";
            await _storage.UploadFileAsync(relativePath, stream);

            /*─ 5. создаём и сохраняем сущность в БД ─────────────────────*/
            var entity = new FileEntity(Guid.NewGuid(),
                                        originalName,
                                        relativePath,
                                        dto.MimeType ?? "application/octet-stream",
                                        0);

            _db.Files.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        
        /// <summary>Копирует <paramref name="src"/> в MemoryStream и возвращает его.</summary>
        private static async Task<MemoryStream> ToMemoryStreamAsync(Stream src, CancellationToken ct = default)
        {
            var ms = new MemoryStream();
            await src.CopyToAsync(ms, 64 * 1024, ct);
            ms.Position = 0;
            return ms;
        }

        /*──────────────────────────── helpers ─────────────────────────────*/

        /// <summary>Получаем исходный поток: из формы или скачиваем из URL.</summary>
        private async Task<Stream> GetStreamAsync(CreateFileDto dto, CancellationToken ct = default)
        {
            /* из multipart-формы */
            if (dto.Stream != null) return dto.Stream;

            /* скачивание по URL */
            if (!Uri.TryCreate(dto.DownloadUrl, UriKind.Absolute, out var uri))
                throw new InvalidOperationException("URL must be absolute http/https.");

            if (!_env.IsDevelopment() &&
                !AllowedHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Host '{uri.Host}' is not allowed.");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(15));

            var resp = await _http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            resp.EnsureSuccessStatusCode();

            if (!_env.IsDevelopment())
            {
                var len = resp.Content.Headers.ContentLength;
                if (len.HasValue && len > MaxRemoteBytes)
                    throw new InvalidOperationException("Remote file too large.");
            }

            var netStream = await resp.Content.ReadAsStreamAsync(ct);
            return _env.IsDevelopment()
                 ? netStream
                 : new LimitedStream(netStream, MaxRemoteBytes);
        }

        /// <summary>Определяем MIME по первым байтам (можно подключить MimeDetective).</summary>
        private static async Task<string> DetectMimeAsync(Stream s)
        {
            var pos = s.CanSeek ? s.Position : 0;
            var inspector = new ContentInspectorBuilder
                            {
                                Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
                            }.Build();
            var result = inspector.Inspect(s);
            if (s.CanSeek) s.Position = pos;
            var mime = result.ByMimeType().FirstOrDefault()?.MimeType ?? "application/octet-stream";
            return mime;
        }
    }
}
