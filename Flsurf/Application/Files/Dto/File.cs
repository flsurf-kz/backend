using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Application.Files.Dto
{
    public class CreateFileDto
    {
        public string? DownloadUrl { get; set; } = null!;
        public Guid? FileId { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? MimeType { get; set; } = "image/jpeg";
        public Stream? Stream { get; set; } = null!;
        /// <summary>
        /// Если true, указывает, что запрос исходит из доверенного внутреннего источника,
        /// и некоторые проверки (например, MIME, ограничения хоста/размера для URL) могут быть пропущены.
        /// Это свойство не может быть установлено из внешних API-запросов из-за его внутреннего сеттера.
        /// </summary>
        public bool Trusted { get; internal set; } = false; // <--- Ключевое изменение
    }

    public class DeleteFileDto
    {
        // DONT REMOVE JSON IGNORE, AND DONT USE THIS SCHEME IN CONTROLLERS DIRECTLY CREATE SCHEMAS
        [JsonIgnore]
        public bool Directly { get; set; } = false;
        public Guid FileId { get; set; }
    }
}
