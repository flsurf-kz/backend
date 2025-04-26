using Flsurf.Presentation.Web.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Files.Entities
{
    public class FileEntity
    {
        [Key, Required]
        public Guid Id { get; set; }

        [Required, MaxLength(256)]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required, MaxLength(128)]
        public string? MimeType { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string? OriginalDownloadUrl { get; set; }

        // Assuming Size is meant to be a numeric type, not GUID.
        public long Size { get; set; } = 0;

        public FileEntity(Guid id, string fileName, string filePath, string? mimeType, long size)
        {
            Id = id;
            FileName = fileName;
            FilePath = filePath;
            MimeType = mimeType;
            Size = size;
        }
    }
}
