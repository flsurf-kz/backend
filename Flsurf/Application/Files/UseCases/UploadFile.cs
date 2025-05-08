using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Domain.Files.Entities;
using Flsurf.Infrastructure.Adapters.FileStorage;
using Microsoft.EntityFrameworkCore;
using MimeDetective;
using MimeDetective.Definitions;
using System.Net;
using System.Security;

namespace Flsurf.Application.Files.UseCases
{
    public class UploadFile : BaseUseCase<CreateFileDto, FileEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageAdapter _fileStorage;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UploadFile> _logger; 

        public UploadFile(IApplicationDbContext dbContext, IFileStorageAdapter fileStorage, IHttpClientFactory httpClient, ILogger<UploadFile> logger)
        {
            _context = dbContext;
            _fileStorage = fileStorage;
            _httpClient = httpClient.CreateClient();
            _logger = logger;
        }

        public async Task<FileEntity> Execute(CreateFileDto dto)
        {
            if (dto.FileId != null)
            {

                _logger.LogInformation("HERE I CUMM, {}", dto.ToString());
                var image = await _context.Files
                    .FirstOrDefaultAsync(x => x.Id == dto.FileId);
                Guard.Against.Null(image, message: $"Image does not exists with id: {dto.FileId}");

                return image;
            }
            Stream? fileStream = dto.Stream;
            if (fileStream == null && !string.IsNullOrEmpty(dto.DownloadUrl))
            {
                // Если Stream не предоставлен, но есть URL, загружаем файл из интернета
                var response = await _httpClient.GetAsync(dto.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                fileStream = await response.Content.ReadAsStreamAsync();
            }

            if (fileStream == null)
            {
                throw new InvalidOperationException("Не удалось получить поток файла.");
            }

            string name;
            if (dto.Name != null)
            {
                name = dto.Name;
            }
            else
            {
                name = Guid.NewGuid().ToString();
            }

            var uniqueFileName = GenerateUniqueFileName(name);
            var filePath = $"files/{uniqueFileName}"; // Пример пути для сохранения файла

            // Сохраняем файл во внешнем хранилище
            await _fileStorage.UploadFileAsync(filePath, fileStream);

            // Создаем новую сущность ImageEntity для сохранения информации о загруженном файле
            var fileEntity = new FileEntity(Guid.NewGuid(), name, filePath, dto.MimeType, 0); // Размер может быть установлен, если доступен

            // Сохраняем сущность в базе данных
            _context.Files.Add(fileEntity);
            await _context.SaveChangesAsync();

            // Возвращаем сущность для дальнейшего использования
            return fileEntity;
        }

        private string GenerateUniqueFileName(string? originalFileName)
        {
            // Пример простой реализации. В реальных сценариях может потребоваться более сложная логика для генерации имени.
            var extension = Path.GetExtension(originalFileName);
            return Guid.NewGuid().ToString() + extension;
        }
    }
}
