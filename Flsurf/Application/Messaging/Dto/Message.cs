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
}
