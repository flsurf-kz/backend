using Flsurf.Application.Common.Models;

namespace Flsurf.Application.Messaging.Dto
{
    public record BookmarkChatDto
    {
        public Guid ChatId;
    }

    public record CreateChatDto {
        public string Name = null!; 
        public string Description = null!;
        public List<Guid> UserIds = []; 
    }

    public record CloseChatDto
    {
        public Guid ChatId; 
    }

    public class GetChatsListDto : InputPagination
    {
        public Guid? UserId { get; set; }
    }
}
