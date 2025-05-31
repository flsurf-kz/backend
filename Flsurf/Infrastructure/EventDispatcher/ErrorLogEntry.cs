using System.Collections;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public class ErrorLogEntry
    {
        public string? ExceptionType { get; set; } = string.Empty; 
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; } = string.Empty;
        public string? Source { get; set; } = string.Empty;
        public string? TargetSiteName { get; set; } = string.Empty; // Упрощенное представление TargetSite
        public IDictionary? Data { get; set; } // Или более типизированный словарь
        public ErrorLogEntry? InnerError { get; set; } // Для InnerException

        public static ErrorLogEntry? FromException(Exception? ex)
        {
            if (ex == null) return null; 
            return new ErrorLogEntry
            {
                ExceptionType = ex.GetType().FullName,
                Message = ex.Message,
                StackTrace = ex.StackTrace, // Может быть урезан для краткости
                Source = ex.Source,
                TargetSiteName = ex.TargetSite?.ToString(), // Упрощенно
                Data = ex.Data.Count > 0 ? new Dictionary<object, object>(ex.Data.Cast<DictionaryEntry>().ToDictionary(de => de.Key, de => de.Value)) : null,
                InnerError = FromException(ex.InnerException)
            };
        }
    }
}
