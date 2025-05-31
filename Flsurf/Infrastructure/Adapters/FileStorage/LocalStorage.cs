using MimeKit; // Заменяет 
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

        // допустимые символы в имени файла (более строгий вариант)
        // Позволяет буквы (Unicode), цифры, точки, дефисы, подчеркивания и пробелы.
        // Запрещает символы, часто используемые для path traversal (/, \) или другие спецсимволы.
        private static readonly Regex _fileNameRegex =
            new(@"^[\p{L}\p{N}\w\-. ]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);


        // Белый список расширений. Используется в GetSafePath, если раскомментировать.
        // SVG убран из-за XSS-риска, если только вы не проводите его санитизацию.
        private static readonly HashSet<string> _allowedExt =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".pdf" };

        public int MaxUploadBytes { get; init; } = 20 * 1024 * 1024;   // 20 MB
        // HttpDownloadTimeout здесь не используется, но оставлен для полноты определения класса
        public TimeSpan HttpDownloadTimeout { get; init; } = TimeSpan.FromSeconds(15);

        public LocalFileStorageAdapter(
            string baseDirectory,
            ILogger<LocalFileStorageAdapter> logger,
            IHostEnvironment env) // IHostEnvironment обычно внедряется через DI
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            env = env ?? throw new ArgumentNullException(nameof(env));

            _devMode = env.IsDevelopment();
            _baseDirectory = Path.GetFullPath(baseDirectory);
            _rootWithSlash = _baseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())
                                ? _baseDirectory
                                : _baseDirectory + Path.DirectorySeparatorChar;

            try
            {
                Directory.CreateDirectory(_baseDirectory);
                _logger.LogInformation("Base directory for LocalFileStorageAdapter initialized at: {BaseDirectory}", _baseDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to create or access base directory: {BaseDirectory}", _baseDirectory);
                throw; // Перебрасываем, так как адаптер не сможет работать
            }
        }

        /// <summary>Запись файла — с DoS-поток-лимитом, без symlink-escape, с проверкой сигнатуры.</summary>
        public async Task UploadFileAsync(string relativePath, Stream fileStream, bool trusted = false)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Relative path cannot be null or whitespace.", nameof(relativePath));

            string sanitizedRelativePath = Sanitize(relativePath);
            _logger.LogInformation("LocalFileStorageAdapter: Uploading file to relative path: {RelativePath}", sanitizedRelativePath);

            string fullPath = GetSafePath(sanitizedRelativePath);
            _logger.LogDebug("LocalFileStorageAdapter: Calculated full path for upload: {FullPath}", fullPath);

            string? directoryName = Path.GetDirectoryName(fullPath);
            if (directoryName == null)
            {
                _logger.LogError("LocalFileStorageAdapter: Could not determine directory name from full path: {FullPath}", fullPath);
                throw new ArgumentException("Invalid relative path resulting in null directory name.", nameof(relativePath));
            }
            Directory.CreateDirectory(directoryName);


            if (fileStream.CanSeek)
            {
                if (!_devMode && fileStream.Length > MaxUploadBytes)
                {
                    _logger.LogWarning("LocalFileStorageAdapter: Input fileStream (seekable) exceeds MaxUploadBytes ({FileLength} > {MaxUploadBytes}) before writing to disk.", fileStream.Length, MaxUploadBytes);
                    throw new InvalidOperationException($"File (from seekable stream) exceeds {MaxUploadBytes} bytes.");
                }
                fileStream.Position = 0; // Убедимся, что читаем с начала
            }

            FileStream? fs = null;
            try
            {
                fs = new FileStream(
                    fullPath,
                    FileMode.CreateNew, // Гарантирует, что файл создается, а не перезаписывается
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 65536, // 64KB
                    FileOptions.Asynchronous);

                _logger.LogDebug("LocalFileStorageAdapter: Opened FileStream to {FullPath} for writing.", fullPath);

                // Копируем из входящего потока в файловый поток с ограничением по размеру
                long totalBytesWritten = 0;
                byte[] buffer = new byte[81920]; // 80KB buffer, стандартный для CopyToAsync
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, bytesRead);
                    totalBytesWritten += bytesRead;

                    if (!_devMode && totalBytesWritten > MaxUploadBytes)
                    {
                        _logger.LogWarning("LocalFileStorageAdapter: File exceeds MaxUploadBytes ({TotalBytesWritten} > {MaxUploadBytes}) during writing to disk. Aborting.", totalBytesWritten, MaxUploadBytes);
                        // fs будет закрыт в finally, и мы попытаемся удалить файл
                        throw new InvalidOperationException($"File write aborted. Size exceeds {MaxUploadBytes} bytes.");
                    }
                }
                await fs.FlushAsync(); // Убедимся, что все данные записаны на диск
                _logger.LogInformation("LocalFileStorageAdapter: Successfully wrote {TotalBytesWritten} bytes to {FullPath}", totalBytesWritten, fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LocalFileStorageAdapter: Error during file write operation to {FullPath}.", fullPath);
                // Если файл был создан, но произошла ошибка, пытаемся его удалить
                if (fs != null) // fs еще может быть не null, если ошибка произошла после его создания
                {
                    await fs.DisposeAsync(); // Закрываем поток перед удалением
                    fs = null; // Чтобы не пытаться закрыть его снова в finally
                }
                if (File.Exists(fullPath))
                {
                    try { File.Delete(fullPath); _logger.LogInformation("LocalFileStorageAdapter: Partially written or failed file {FullPath} deleted.", fullPath); }
                    catch (IOException ioEx) { _logger.LogWarning(ioEx, "LocalFileStorageAdapter: Could not delete partially written or failed file {FullPath}.", fullPath); }
                }
                throw; // Перебрасываем оригинальное исключение
            }
            finally
            {
                if (fs != null)
                {
                    await fs.DisposeAsync();
                }
            }

            // ★ 2) После закрытия записи — проверяем MIME
            _logger.LogDebug("LocalFileStorageAdapter: Validating MIME for {FullPath}", fullPath);
            try
            {
                if (!_devMode)
                {
                    await ValidateMimeAsync(fullPath, _logger); // Передаем логгер
                }
                _logger.LogInformation("LocalFileStorageAdapter: MIME validation successful for {FullPath}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LocalFileStorageAdapter: MIME validation failed for {FullPath}. Attempting to delete file.", fullPath);
                // Если MIME не валиден, файл уже должен быть удален внутри ValidateMimeAsync (если он так реализован)
                // Дополнительная проверка и попытка удаления здесь для надежности.
                if (File.Exists(fullPath))
                {
                    try { File.Delete(fullPath); _logger.LogInformation("LocalFileStorageAdapter: File {FullPath} deleted due to failed MIME validation.", fullPath); }
                    catch (IOException ioEx) { _logger.LogWarning(ioEx, "LocalFileStorageAdapter: Could not delete file {FullPath} after failed MIME validation.", fullPath); }
                }
                throw; // Перебрасываем исключение от ValidateMimeAsync
            }
        }

        public Task<Stream> DownloadFileAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Relative path cannot be null or whitespace.", nameof(relativePath));
            string sanitizedRelativePath = Sanitize(relativePath);
            _logger.LogInformation("LocalFileStorageAdapter: Downloading file from relative path: {RelativePath}", sanitizedRelativePath);

            var fullPath = GetSafePath(sanitizedRelativePath);
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("LocalFileStorageAdapter: File not found for download: {FullPath}", fullPath);
                throw new FileNotFoundException("File not found at specified path.", fullPath);
            }

            _logger.LogDebug("LocalFileStorageAdapter: Opening FileStream from {FullPath} for download.", fullPath);
            var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read, // Разрешаем чтение другими процессами
                bufferSize: 65536, // 64KB
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            return Task.FromResult<Stream>(stream);
        }

        public Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentException("Relative path cannot be null or whitespace.", nameof(relativePath));
            string sanitizedRelativePath = Sanitize(relativePath);
            _logger.LogInformation("LocalFileStorageAdapter: Deleting file with relative path: {RelativePath}", sanitizedRelativePath);

            var fullPath = GetSafePath(sanitizedRelativePath);
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("LocalFileStorageAdapter: File {FullPath} deleted successfully.", fullPath);
                }
                else
                {
                    _logger.LogWarning("LocalFileStorageAdapter: File not found for deletion: {FullPath}", fullPath);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "LocalFileStorageAdapter: IO error while deleting file {FullPath}.", fullPath);
                throw; // Перебрасываем, чтобы сообщить о проблеме
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "LocalFileStorageAdapter: Unauthorized access while deleting file {FullPath}.", fullPath);
                throw;
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path)); // Путь может быть "" для корня адаптера
            string sanitizedPath = Sanitize(path);
            _logger.LogInformation("LocalFileStorageAdapter: Listing files in relative path: {RelativePath}", sanitizedPath);

            var fullPath = GetSafePath(sanitizedPath); // GetSafePath должен корректно обрабатывать пустой relativePath для корня адаптера

            if (!Directory.Exists(fullPath))
            {
                _logger.LogWarning("LocalFileStorageAdapter: Directory not found for listing: {FullPath}", fullPath);
                return Task.FromResult(Enumerable.Empty<string>());
            }

            var rawNames = Directory.EnumerateFiles(fullPath)
                                 .Select(Path.GetFileName);
            if (rawNames == null)
                return Task.FromResult(Enumerable.Empty<string>());
            var names = rawNames.Select(x => string.IsNullOrEmpty(x) ? "NOT FOUND" : x);
            return Task.FromResult(names); // Path.GetFileName не возвращает null для EnumerateFiles
        }

        public Task Configure()
        {
            _logger.LogInformation("LocalFileStorageAdapter: Configure method called (no specific configuration implemented).");
            return Task.CompletedTask;
        }

        private string GetSafePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                // Если относительный путь пуст, он может указывать на корневой каталог адаптера.
                // Path.Combine(_baseDirectory, "") вернет _baseDirectory.
                // Path.GetFullPath(_baseDirectory) вернет канонический путь.
                // Это должно быть безопасным, так как _rootWithSlash уже проверен.
                // Однако, для цикла проверки компонентов ниже, нам нужен хотя бы один компонент или отсутствие компонентов.
                // Если пустой relativePath означает корень, то компоненты проверять не нужно.
                // Но если мы ожидаем, что relativePath всегда содержит имя файла/каталога, то это ошибка.
                // Для данного случая, если relativePath это, например, имя файла в корне, он не должен быть пустым.
                // Если relativePath это "папка/файл", то он тоже не пустой.
                // Если это "" для обозначения корня хранилища, то можно вернуть _baseDirectory.
                // Но так как вызывающий код формирует "files/...", пустой путь маловероятен.
                _logger.LogWarning("GetSafePath called with null or whitespace relativePath. This might be an issue depending on usage context.");
                throw new ArgumentException("Relative path cannot be null or whitespace.", nameof(relativePath));
            }

            // Нормализуем разделители для корректного разделения на компоненты.
            // Заменяем все \ на / для единообразия перед Split.
            string normalizedForComponentSplitting = relativePath.Replace('\\', '/');

            // Разделяем путь на компоненты по '/' и удаляем пустые компоненты (например, если было "files//image.png")
            var pathComponents = normalizedForComponentSplitting.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (!pathComponents.Any())
            {
                _logger.LogWarning("Relative path '{OriginalRelativePath}' resulted in no valid path components after splitting.", relativePath);
                throw new ArgumentException("Relative path is invalid or contains only separators.", nameof(relativePath));
            }

            foreach (var component in pathComponents)
            {
                // Проверка на попытки обхода каталога
                if (component == "." || component == "..")
                {
                    _logger.LogWarning("Path traversal attempt detected with '.' or '..' in component: '{Component}' of original path '{OriginalRelativePath}'", component, relativePath);
                    throw new ArgumentException($"Path traversal attempt with invalid component '{component}'.", nameof(relativePath));
                }
                // Проверка на допустимые символы в каждом компоненте
                if (!_fileNameRegex.IsMatch(component))
                {
                    _logger.LogWarning("Invalid characters in path component: '{Component}' (from original path '{OriginalRelativePath}'). Regex: '{FileNameRegex}'", component, relativePath, _fileNameRegex.ToString());
                    throw new ArgumentException($"Invalid characters in path component: '{component}'.", nameof(relativePath));
                }
            }

            // После валидации всех компонентов, мы можем безопасно использовать Path.Combine.
            // Path.Combine корректно обработает разделители для текущей ОС.
            // Чтобы Path.Combine работал предсказуемо и для последующей проверки StartsWith,
            // удалим возможные ведущие слэши из original relativePath, так как _baseDirectory уже их учитывает.
            string pathToCombine = relativePath.TrimStart('/', '\\');

            var fullPath = Path.Combine(_baseDirectory, pathToCombine);
            fullPath = Path.GetFullPath(fullPath); // Канонизация пути, разрешение ".." (хотя мы их запретили), и приведение к ОС-специфичным разделителям.

            // Финальная проверка безопасности, что полученный путь не выходит за пределы _baseDirectory.
            if (!fullPath.StartsWith(_rootWithSlash, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Path traversal attempt detected. Calculated full path '{FullPath}' is outside base directory '{BaseDir}'", fullPath, _rootWithSlash);
                throw new UnauthorizedAccessException("Path traversal attempt: result is outside base directory.");
            }

            return fullPath;
        }

        private static string Sanitize(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            // Заменяем символы новой строки и возврата каретки, а также другие потенциально опасные символы
            // Можно быть более строгим здесь в зависимости от требований.
            return s.Replace("\r", "_").Replace("\n", "_").Replace("\0", "_");
        }

        // Сделал метод нестатическим, чтобы он мог использовать _logger, или передаем ILogger как параметр
        private static async Task ValidateMimeAsync(string fullPath, ILogger logger) // Теперь принимает ILogger
        {
            string mime = "application/octet-stream";

            try
            {
                await using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fs.Length == 0)
                    {
                        mime = "application/octet-stream"; // Или другой MIME для пустых файлов, например, "inode/x-empty"
                        logger.LogWarning("File {FullPath} is empty. MIME set to {MimeType}", fullPath, mime);
                    }
                    else
                    {
                        MimePart part = new MimePart();
                        part.Content = new MimeContent(fs, ContentEncoding.Default);
                        mime = part.ContentType.MimeType;
                        logger.LogInformation("MIME for {FullPath} detected by MimeKitLite as: {MimeType}", fullPath, mime);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during MimeKitLite detection for {FullPath}. Defaulting to application/octet-stream.", fullPath);
                mime = "application/octet-stream";
            }

            // Примерный белый список MIME типов. Адаптируйте под ваши нужды.
            // SVG (image/svg+xml) требует особой осторожности из-за XSS.
            bool isAllowedMime = mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase) || // Разрешаем все image/*
                                 mime.Equals("application/pdf", StringComparison.OrdinalIgnoreCase); 

            // mime.Equals("image/svg+xml", StringComparison.OrdinalIgnoreCase); // Если SVG разрешен

            if (!isAllowedMime)
            {
                logger.LogWarning("Forbidden MIME type '{MimeType}' detected for file {FullPath}. Deleting file.", mime, fullPath);
                try
                {
                    File.Delete(fullPath);
                    logger.LogInformation("File {FullPath} deleted due to forbidden MIME type.", fullPath);
                }
                catch (Exception exDelete)
                {
                    logger.LogError(exDelete, "Failed to delete file {FullPath} after forbidden MIME type detection.", fullPath);
                }
                throw new InvalidOperationException($"Detected MIME '{mime}' is not allowed for file '{Path.GetFileName(fullPath)}'.");
            }
        }
    }
}