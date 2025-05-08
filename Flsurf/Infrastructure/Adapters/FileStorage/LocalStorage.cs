using System.Text.RegularExpressions;

namespace Flsurf.Infrastructure.Adapters.FileStorage
{
    public class LocalFileStorageAdapter : IFileStorageAdapter
    {
        private readonly string _baseDirectory;
        private static readonly Regex _fileNameRegex =
            new(@"^[\w\-. ]+$", RegexOptions.Compiled);          // допустимые символы
        private static readonly HashSet<string> _allowedExt =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".png", ".pdf", ".svg" /* … */ };
        private ILogger _logger; 

        public LocalFileStorageAdapter(string baseDirectory, ILogger<IFileStorageAdapter> logger)
        {
            _logger = logger;
            _baseDirectory = Path.GetFullPath(baseDirectory);
            Directory.CreateDirectory(_baseDirectory);          // гарантируем существование
        }

        public async Task UploadFileAsync(string relativePath, Stream fileStream)
        {
            _logger.LogInformation("Creating a file with a relative path: {}", relativePath);

            var fullPath = GetSafePath(relativePath); 

            var directory = Path.GetDirectoryName(fullPath)!;
            Directory.CreateDirectory(directory);

            await using var file = new FileStream(fullPath, FileMode.CreateNew,
                                                  FileAccess.Write, FileShare.None,
                                                  64 * 1024, useAsync: true);
            await fileStream.CopyToAsync(file, 64 * 1024);
        }

        public async Task<Stream> DownloadFileAsync(string relativePath)
        {

            _logger.LogInformation("Downloading a file with a relative path: {}", relativePath);
            var fullPath = GetSafePath(relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found.");

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read,
                                  FileShare.Read, 64 * 1024, useAsync: true);
        }

        public Task DeleteFileAsync(string relativePath)
        {

            _logger.LogInformation("Deleting a file with a relative path: {}", relativePath);

            var fullPath = GetSafePath(relativePath);
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            var fullPath = Path.Combine(_baseDirectory, path);
            var files = Directory.EnumerateFiles(fullPath);
            return Task.FromResult(files);
        }

        public Task Configure()
        {
            return Task.CompletedTask;
        }

        private string GetSafePath(string relativePath)
        {
            // 1) базовая валидация имени
            var fileName = Path.GetFileName(relativePath);
            if (!_fileNameRegex.IsMatch(fileName))
                throw new ArgumentException($"Invalid characters in file name. name: {fileName}") ;

            // 2) whitelist расширений
            var ext = Path.GetExtension(fileName);
            if (!_allowedExt.Contains(ext))
                throw new ArgumentException($"File type not allowed. ext: {ext}");

            // 3) нормализуем и проверяем, что путь остаётся внутри baseDir
            var fullPath = Path.GetFullPath(Path.Combine(_baseDirectory, relativePath));
            if (!fullPath.StartsWith(_baseDirectory, StringComparison.Ordinal))
                throw new UnauthorizedAccessException("Path traversal attempt detected.");

            return fullPath;
        }
    }
}
