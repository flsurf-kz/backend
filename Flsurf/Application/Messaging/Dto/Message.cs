using Flsurf.Application.Files.Dto;

namespace Flsurf.Application.Messaging.Dto
{
    public record UpdateMessageDto {
        public Guid MessageId;
        public string? Text;
        public ICollection<CreateFileDto>? Photos; 
    }
    public record SendMessageDto { }
    public record PinMessageDto { }
    public record UnpinMessageDto { }
    public record GetMessagesListDto { }
}
