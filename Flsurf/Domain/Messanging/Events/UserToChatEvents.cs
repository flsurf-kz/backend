using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatBookmarked(UserToChatEntity userToChat) : BaseEvent
    {
        public Guid UserId { get; } = userToChat.UserId;
        public Guid ChatId { get; } = userToChat.ChatId; 
    }
}
