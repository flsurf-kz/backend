using Flsurf.Application.Files.Dto;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Messaging.Dto
{
    public class UpdateMessageDto {
        public Guid MessageId { get; set; }
        public string? Text { get; set; }
        public ICollection<CreateFileDto>? Files { get; set; }
    }
    public class SendMessageDto {
        [Required]
        public Guid ChatId { get; set; }
        public string? Text { get; set; }
        public ICollection<CreateFileDto>? Files { get; set; }
        public Guid? replyToMsg { get; set; }
    }
    public class PinMessageDto
    {
        public Guid MessageId { get; set; }
    }

    public class UnpinMessageDto { }
    public class GetMessagesListDto {
        [Required]
        public Guid ChatId { get; set; }
        public int Starts { get; set; } = 0;  
        // override 
        public int Ends { get; set; } = 20;
    }

    // websocket messages 
    public sealed class FileAttachmentDto         // лёгкая «визитка» файла
    {
        public Guid Id { get; init; }
        public string FileName { get; init; } = default!;
        public string? MimeType { get; init; } = default!;
        public long Size { get; init; }
        public string FilePath { get; set; } = null!; 
    }

    public sealed class MessageContract           // заменяем record → class
    {
        public Guid Id { get; init; }
        public Guid SenderId { get; init; }
        public string Text { get; init; } = default!;
        public DateTime CreatedAt { get; init; }

        public IReadOnlyList<FileAttachmentDto> Files { get; init; } = Array.Empty<FileAttachmentDto>();
    }
}
