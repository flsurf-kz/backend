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

    public record UpdateChatDto {
        public Guid ChatId { get; set; }
    }

    public record GetUserChats { }

    public record MarkAsReadDto { }

    public record KickMemberDto { }

    public record InviteMemberDto { }

    public record GetUserChatsDto { }

    public record DeleteMessageDto { 
        public Guid MessgeId { get; set; }
    }
}
