using Flsurf.Application.Common.Models;
using Flsurf.Domain.Messanging.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Messaging.Dto
{
    public class BookmarkChatDto
    {
        public Guid ChatId { get; set; }
    }

    public class CreateChatDto {
        [Required]
        public string Name { get; set; } = null!; 
        public string Description { get; set; } = null!;
        public List<Guid> UserIds { get; set; } = [];
        public ChatTypes type { get; set; }
    }

    public class CloseChatDto
    {
        public Guid ChatId { get; set; }
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


    public class GetUserChats { }

    public class MarkAsReadDto { 
        public Guid ChatId { get; set; }
    }

    public class KickMemberDto {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
    }

    public class InviteMemberDto {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public bool Owner { get; set; }
    }

    public class GetUserChatsDto { }

    public class DeleteMessageDto { 
        public Guid MessgeId { get; set; }
    }
}
