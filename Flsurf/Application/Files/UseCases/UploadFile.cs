using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases; // Предполагается, что Guard и BaseUseCase находятся здесь или в общих моделях
using Flsurf.Application.Files.Dto;
using Flsurf.Domain.Files.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Flsurf.Infrastructure.Adapters.FileStorage;

// Если Guard.Against.Null приходит из определенной библиотеки, убедитесь, что она подключена.
// Например, Ardalis.GuardClauses: Install-Package Ardalis.GuardClauses

namespace Flsurf.Application.Files.UseCases
{
    /// <summary>
    /// Юз-кейс загрузки файла (из URL или из запроса).
    /// </summary>
    public class UploadFile : BaseUseCase<CreateFileDto, FileEntity>
    {
        private const int MaxRemoteBytes = 20 * 1024 * 1024; // 20 MB

        // «белые» хосты для скачивания (Prod-режим)
        private static readonly string[] AllowedHosts =
        {
            "cdn.example.com", // Замените на ваши реальные доверенные хосты
            "storage.googleapis.com",
            "card-banks.ru", // Добавлен из вашего начального кода
            "cdn-icons-png.flaticon.com" // Добавлен из вашего начального кода
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
            _http = httpFactory.CreateClient("Default_Timeout_Increased"); // Рекомендуется использовать именованный клиент с настроенным таймаутом
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
                Guard.Against.Null(ent, nameof(ent), $"File entity does not exist: {id}"); // Используем nameof для имени параметра
                return ent!;
            }

            /*─ 2. получаем поток ─────────────────────────────────────────*/
            _log.LogDebug("Attempting to get stream for DTO: Name='{DtoName}', Url='{DtoUrl}'", dto.Name, dto.DownloadUrl);
            await using var baseStream = await GetStreamAsync(dto);
            Stream stream = baseStream;

            if (!stream.CanSeek)
            {
                _log.LogDebug("Stream is not seekable. Copying to MemoryStream.");
                stream = await ToMemoryStreamAsync(stream); // Копирует и устанавливает Position = 0
            }
            else
            {
                stream.Position = 0;
            }
            _log.LogDebug("Stream prepared. Length: {StreamLength}, CanSeek: {CanSeek}", stream.Length, stream.CanSeek);


            /*─ 3. определяем и сверяем MIME ──────────────────────────────*/
            _log.LogDebug("Detecting MIME for stream..."); // Ваш оригинальный лог
            string detectedMime = await DetectMimeAsync(stream); // Ваш оригинальный вызов
            //string detectedMime = "image/png";  // Ваша закомментированная строка
            _log.LogInformation("Detected MIME: {DetectedMime} for DTO Name: {DtoName}", detectedMime, dto.Name); // Ваш оригинальный лог
            string mimeToStore;

            if (dto.Trusted) // <--- ИЗМЕНЕННАЯ ЛОГИКА ЗДЕСЬ
            {
                // Если это внутренний/доверенный вызов (BypassExternalChecks = true)
                if (!string.IsNullOrWhiteSpace(dto.MimeType))
                {
                    // Используем предоставленный MimeType без строгой сверки с detectedMime
                    mimeToStore = dto.MimeType;
                    _log.LogInformation("[BypassActive] Using provided MimeType '{MimeType}' for file (DTO Name: {DtoName}). Strict MIME validation skipped.", mimeToStore, dto.Name);
                }
                else
                {
                    // Если MimeType не предоставлен даже для внутреннего вызова, используем определенный.
                    // Строгая сверка не производится.
                    mimeToStore = detectedMime; 
                    _log.LogInformation("[BypassActive] MimeType not provided in DTO. Using detected/defaulted MimeType '{MimeType}' for file (DTO Name: {DtoName}). Strict MIME validation skipped.", mimeToStore, dto.Name);
                }
            }
            else // Стандартная логика для внешних вызовов (dto.BypassExternalChecks == false)
            {
                // Эта часть соответствует вашей оригинальной логике
                if (!string.IsNullOrWhiteSpace(dto.MimeType))
                {
                    if (!detectedMime.Equals(dto.MimeType, StringComparison.OrdinalIgnoreCase))
                    {
                        _log.LogWarning("Declared MIME '{DeclaredMime}' does not match detected MIME '{DetectedMime}' for file (DTO Name: {DtoName})", dto.MimeType, detectedMime, dto.Name);
                        throw new InvalidOperationException($"Declared MIME '{dto.MimeType}' " +
                                                            $"does not match real detected MIME '{detectedMime}'.");
                    }
                    mimeToStore = dto.MimeType;
                    _log.LogInformation("Using declared MIME '{MimeType}' for file (DTO Name: {DtoName}) after successful validation against detected '{DetectedMime}'", mimeToStore, dto.Name, detectedMime);
                }
                else
                {
                    mimeToStore = detectedMime; 
                    _log.LogInformation("Using detected MIME '{MimeType}' for file (DTO Name: {DtoName})", mimeToStore, dto.Name);
                }
            }

            if (stream.CanSeek)
            {
                stream.Position = 0; // Сброс позиции перед передачей в хранилище
            }


            /*─ 4. сохраняем в хранилище ─────────────────────────────────*/
            var originalName = string.IsNullOrWhiteSpace(dto.Name)
                                 ? Guid.NewGuid().ToString()
                                 : Path.GetFileName(dto.Name!); // Используем только имя файла

            var fileExtension = Path.GetExtension(originalName);
            // Логика для добавления расширения на основе MIME, если необходимо, может быть здесь.
            // Например: if (string.IsNullOrEmpty(fileExtension) && mimeToStore == "image/png") fileExtension = ".png";

            var relativePath = $"files/{Guid.NewGuid():N}{fileExtension}";

            _log.LogInformation("Uploading file {OriginalName} to {RelativePath} with MIME {MimeType}", originalName, relativePath, mimeToStore);
            await _storage.UploadFileAsync(relativePath, stream);


            /*─ 5. создаём и сохраняем сущность в БД ─────────────────────*/
            long fileSize = 0;
            if (stream.CanSeek)
            {
                try { fileSize = stream.Length; }
                catch (ObjectDisposedException) { _log.LogWarning("Stream was disposed before length could be read for FileEntity."); }
            }
            else
            {
                _log.LogWarning("Cannot determine stream length for FileEntity because stream is not seekable (original was {BaseStreamType})", baseStream.GetType().Name);
                // Можно попытаться получить ContentLength из DTO, если бы он там был.
            }


            var entity = new FileEntity(Guid.NewGuid(),
                                         originalName,
                                         relativePath,
                                         mimeToStore,
                                         fileSize);

            _db.Files.Add(entity);
            await _db.SaveChangesAsync();
            _log.LogInformation("File entity {FileId} created for {OriginalName} with MIME {MimeType}, Size: {FileSize}", entity.Id, originalName, mimeToStore, fileSize);
            return entity;
        }

        /// <summary>
        /// Копирует <paramref name="src"/> в MemoryStream и возвращает его. Позиция в MemoryStream будет 0.
        /// </summary>
        private static async Task<MemoryStream> ToMemoryStreamAsync(Stream src, CancellationToken ct = default)
        {
            var ms = new MemoryStream();
            if (src.CanSeek)
            {
                src.Position = 0; // Убедимся, что копируем с начала
            }
            await src.CopyToAsync(ms, 81920, ct); // Стандартный размер буфера CopyToAsync
            ms.Position = 0;
            return ms;
        }

        /*──────────────────────────── helpers ─────────────────────────────*/

        /// <summary>
        /// Получаем исходный поток: из формы или скачиваем из URL.
        /// </summary>
        private async Task<Stream> GetStreamAsync(CreateFileDto dto, CancellationToken ct = default)
        {
            if (dto.Stream != null)
            {
                _log.LogInformation("Getting stream from DTO for file {FileName}", dto.Name);
                return dto.Stream;
            }

            _log.LogInformation("Downloading stream from URL {DownloadUrl} for file {FileName}", dto.DownloadUrl, dto.Name);
            if (string.IsNullOrWhiteSpace(dto.DownloadUrl) || !Uri.TryCreate(dto.DownloadUrl, UriKind.Absolute, out var uri))
                throw new InvalidOperationException("DownloadUrl must be a valid absolute http/https URL.");

            if (!uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) && !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("URL scheme must be http or https.");


            if (!_env.IsDevelopment() &&
                !AllowedHosts.Any(h => uri.Host.EndsWith(h, StringComparison.OrdinalIgnoreCase) || uri.Host.Equals(h, StringComparison.OrdinalIgnoreCase))) // Проверка на поддомены и точное совпадение
                throw new InvalidOperationException($"Host '{uri.Host}' is not allowed for download.");

            // Увеличиваем таймаут для HttpClient, если он не настроен глобально
            // var client = _httpFactory.CreateClient("FileDownloadClient"); // Предпочтительно использовать именованный клиент

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            // Таймаут для всего запроса, включая получение заголовков и чтение части контента
            // Если общий таймаут HttpClient меньше, он сработает раньше.
            cts.CancelAfter(_http.Timeout > TimeSpan.FromSeconds(15) ? _http.Timeout : TimeSpan.FromSeconds(15));


            HttpResponseMessage resp;
            try
            {
                resp = await _http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                resp.EnsureSuccessStatusCode();
            }
            catch (TaskCanceledException ex) when (cts.IsCancellationRequested && !ct.IsCancellationRequested)
            {
                _log.LogError(ex, "Download from {DownloadUrl} timed out.", dto.DownloadUrl);
                throw new TimeoutException($"Download from '{dto.DownloadUrl}' timed out.", ex);
            }
            catch (HttpRequestException ex)
            {
                _log.LogError(ex, "HTTP request to {DownloadUrl} failed.", dto.DownloadUrl);
                throw; // Перебрасываем оригинальное исключение
            }


            var len = resp.Content.Headers.ContentLength;
            _log.LogInformation("Successfully connected to URL {DownloadUrl}. Content-Type: {ContentTypeHeader}, Content-Length: {ContentLengthHeader}", dto.DownloadUrl, resp.Content.Headers.ContentType, len ?? -1L);

            if (!_env.IsDevelopment())
            {
                if (len.HasValue && len.Value > MaxRemoteBytes)
                {
                    _log.LogWarning("Remote file at {DownloadUrl} is too large ({FileLength} bytes). Max allowed: {MaxBytes}", dto.DownloadUrl, len.Value, MaxRemoteBytes);
                    throw new InvalidOperationException($"Remote file too large ({len.Value} bytes). Max allowed: {MaxRemoteBytes} bytes.");
                }
            }

            var netStream = await resp.Content.ReadAsStreamAsync(cts.Token); // Используем cts.Token

            if (_env.IsDevelopment() || !len.HasValue || len.Value == 0) // Если разработчик или длина неизвестна/ноль
            {
                _log.LogDebug("Returning raw network stream for {DownloadUrl} (DevMode or unknown/zero length)", dto.DownloadUrl);
                return netStream;
            }
            else // Для продакшена с известной длиной, не превышающей MaxRemoteBytes
            {
                // Если длина известна и не превышает лимит, LimitedStream может быть избыточен,
                // но он все равно обеспечивает защиту, если ContentLength был неверным.
                _log.LogDebug("Returning LimitedStream for {DownloadUrl} with limit {MaxBytes}", dto.DownloadUrl, MaxRemoteBytes);
                return new LimitedStream(netStream, MaxRemoteBytes); // Убедитесь, что LimitedStream корректно определен
            }
        }

        /// <summary>
        /// Определяем MIME по содержимому потока с использованием MimeKitLite.
        /// </summary>
        private static Task<string> DetectMimeAsync(Stream s)
        {
            if (s == null)
            {
                // _log недоступен в статическом методе напрямую
                Console.WriteLine("[Static DetectMimeAsync] Error: Stream is null."); // Временное логирование
                throw new ArgumentNullException(nameof(s));
            }
            if (!s.CanRead)
            {
                Console.WriteLine("[Static DetectMimeAsync] Error: Stream is not readable.");
                throw new ArgumentException("Stream is not readable.", nameof(s));
            }

            long originalPosition = -1;
            if (s.CanSeek)
            {
                originalPosition = s.Position;
                s.Position = 0;
            }

            string detectedMimeType;
            try
            {
                MimePart part = new MimePart();
                // Передаем поток в MimeContent. MimeKit попытается определить тип.
                part.Content = new MimeContent(s, ContentEncoding.Default);
                detectedMimeType = part.ContentType.MimeType;

                // Если MimeKit вернул application/octet-stream, а поток пустой, это нормально.
                // Если поток не пустой, а MimeKit не смог определить, он оставит application/octet-stream.
                if (detectedMimeType == "application/octet-stream" && s.Length > 0)
                {
                    Console.WriteLine($"[Static DetectMimeAsync] MimeKitLite detected 'application/octet-stream' for a non-empty stream (Length: {s.Length}). This might indicate an unknown file type or a very generic binary file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Static DetectMimeAsync] MimeKitLite detection error: {ex.Message}. Falling back to application/octet-stream.");
                detectedMimeType = "application/octet-stream";
            }
            finally
            {
                if (s.CanSeek && originalPosition != -1)
                {
                    try { s.Position = originalPosition; }
                    catch (ObjectDisposedException) { /* Поток мог быть закрыт MimeContent, если он был владельцем */ }
                }
            }
            return Task.FromResult(detectedMimeType);
        }
    }
}