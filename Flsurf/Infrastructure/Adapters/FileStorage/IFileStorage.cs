﻿namespace Flsurf.Infrastructure.Adapters.FileStorage
{
    public interface IFileStorageAdapter
    {
        // Загружает файл в хранилище
        Task UploadFileAsync(string path, Stream fileStream, bool trusted = false);

        // Скачивает файл из хранилища
        Task<Stream> DownloadFileAsync(string path);

        // Удаляет файл из хранилища
        Task DeleteFileAsync(string path);

        // Перечисляет файлы в директории
        Task<IEnumerable<string>> ListFilesAsync(string path);
        Task Configure();
    }
}
