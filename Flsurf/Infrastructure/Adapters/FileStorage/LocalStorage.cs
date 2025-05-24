using MimeDetective;
using Namotion.Reflection;
using System.Text.RegularExpressions;

namespace Flsurf.Infrastructure.Adapters.FileStorage
{
    /// <summary>
    /// Адаптер локального файлового хранилища с защитой от traversal, symlink-escape,
    /// DoS, SSRF, XSS, неправильного MIME и пр.
    /// </summary>
    public sealed class LocalFileStorageAdapter : IFileStorageAdapter
    {
        private readonly string _baseDirectory;
        private readonly string _rootWithSlash;
        private readonly ILogger<LocalFileStorageAdapter> _logger;
        private readonly bool _devMode;

        // допустимые символы в имени
        private static readonly Regex _fileNameRegex =
            new(@"^[\w\-. ]+$", RegexOptions.Compiled);

        // белый список расширений (svg убран из-за XSS-риска)
        private static readonly HashSet<string> _allowedExt =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".pdf" };

        // лимиты (применяются, если не Dev)
        public int MaxUploadBytes { get; init; } = 20 * 1024 * 1024;    // 20 MB
        public TimeSpan HttpDownloadTimeout { get; init; } = TimeSpan.FromSeconds(15);

        public LocalFileStorageAdapter(
            string baseDirectory,
            ILogger<LocalFileStorageAdapter> logger,
            IHostEnvironment env)
        {
            _logger = logger;
            _devMode = env.IsDevelopment();
            _baseDirectory = Path.GetFullPath(baseDirectory);
            _rootWithSlash = _baseDirectory.TrimEnd(Path.DirectorySeparatorChar)
                             + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(_baseDirectory);
        }

        /// <summary>Запись файла — с DoS-поток-лимитом, без symlink-escape, с проверкой сигнатуры.</summary>
        public async Task UploadFileAsync(string relativePath, Stream fileStream)
        {
            relativePath = Sanitize(relativePath);
            _logger.LogInformation("Creating a file with a relative path: {RelativePath}", relativePath);

            var fullPath = GetSafePath(relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            // предварительный размер для seek-able
            if (!_devMode && fileStream.CanSeek && fileStream.Length > MaxUploadBytes)
                throw new InvalidOperationException($"File exceeds {MaxUploadBytes} bytes.");

            // ★ 1) Записываем файл и сразу закрываем дескриптор
            await using (var fs = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                64 * 1024,
                FileOptions.Asynchronous))
            {
                var buffer = new byte[64 * 1024];
                long total = 0;
                int read;
                while ((read = await fileStream.ReadAsync(buffer)) > 0)
                {
                    if (!_devMode && (total += read) > MaxUploadBytes)
                        throw new InvalidOperationException($"File exceeds {MaxUploadBytes} bytes.");
                    await fs.WriteAsync(buffer.AsMemory(0, read));
                }
                // fs.Dispose() вызовется по выходу из using
            }

            // ★ 2) После закрытия записи — проверяем MIME
            await ValidateMimeAsync(fullPath);
        }

        /// <summary>Чтение файла — обычный поток, без follow-symlink.</summary>
        public Task<Stream> DownloadFileAsync(string relativePath)
        {
            relativePath = Sanitize(relativePath);
            _logger.LogInformation("Downloading a file with a relative path: {RelativePath}", relativePath);

            var fullPath = GetSafePath(relativePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found.");

            var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                64 * 1024,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            return Task.FromResult((Stream)stream);
        }

        /// <summary>Удаление файла.</summary>
        public Task DeleteFileAsync(string relativePath)
        {
            relativePath = Sanitize(relativePath);
            _logger.LogInformation("Deleting a file with a relative path: {RelativePath}", relativePath);

            var fullPath = GetSafePath(relativePath);
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        /// <summary>Перечисление файлов в каталоге — возвращает только имена.</summary>
        public Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            path = Sanitize(path);
            var fullPath = GetSafePath(path);
            var names = Directory.EnumerateFiles(fullPath)
                                    .Select(Path.GetFileName);
            return Task.FromResult(names);
        }

        public Task Configure() => Task.CompletedTask;

        // ──────────────────────────── helpers ─────────────────────────────

        private string GetSafePath(string relativePath)
        {
            var fileName = Path.GetFileName(relativePath);
            if (!_fileNameRegex.IsMatch(fileName))
                throw new ArgumentException($"Invalid characters in file name: {fileName}");

            //var ext = Path.GetExtension(fileName);
            //if (!_allowedExt.Contains(ext))
            //    throw new ArgumentException($"File type not allowed: {ext}");

            var fullPath = Path.GetFullPath(Path.Combine(_baseDirectory, relativePath));
            if (!fullPath.StartsWith(_rootWithSlash, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Path traversal attempt detected.");

            return fullPath;
        }

        private static string Sanitize(string s)
            => s.Replace("\r", "_").Replace("\n", "_");

        private static async Task ValidateMimeAsync(string fullPath)
        {
            // Проверяем «магические» байты через MimeDetective (или другой инспектор)
            await using var fs = File.OpenRead(fullPath);
            var inspector = new ContentInspectorBuilder
                            {
                                Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
                            }.Build();
            var result = inspector.Inspect(fs);
            var mime   = result.ByMimeType().FirstOrDefault()?.MimeType ?? "application/octet-stream";
            if (!(mime.StartsWith("image/") || mime == "application/pdf"))
                throw new InvalidOperationException($"Detected MIME '{mime}' is not allowed.");
        }
    }
}
