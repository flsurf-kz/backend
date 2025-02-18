using Flsurf.Application.Common.Models;
using Flsurf.Domain.Messanging.Enums;
using System.ComponentModel.DataAnnotations;

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
        public ChatTypes type; 
    }

    public record CloseChatDto
    {
        public Guid ChatId; 
    }

    public class GetChatsListDto : InputPagination
    {
        public Guid? UserId { get; set; }
    }

    public class UpdateChatDto
    {
        [Required]
        public Guid ChatId { get; set; }
        public string? Name { get; set; }
        public bool? IsTextingAllowed { get; set; }
        public bool? IsArchived { get; set; }
    }


    public record GetUserChats { }

    public record MarkAsReadDto { }

    public record KickMemberDto {
        public Guid ChatId;
        public Guid UserId;
    }

    public record InviteMemberDto {
        public Guid ChatId;
        public Guid UserId; 
    }

    public record GetUserChatsDto { }

    public record DeleteMessageDto { 
        public Guid MessgeId { get; set; }
    }
}
