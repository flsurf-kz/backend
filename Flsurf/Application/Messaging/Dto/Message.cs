using Flsurf.Application.Files.Dto;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Messaging.Dto
{
    public record UpdateMessageDto {
        public Guid MessageId;
        public string? Text;
        public ICollection<CreateFileDto>? Files; 
    }
    public record SendMessageDto {
        [Required]
        public Guid ChatId;
        public string? Text;
        public ICollection<CreateFileDto>? Files;
    }
    public class PinMessageDto
    {
        public Guid MessageId { get; set; }
    }

    public record UnpinMessageDto { }
    public record GetMessagesListDto {
        [Required]
        public Guid ChatId;
        public int starts = 0;
        // override 
        public int ends = 20;
    }
}
