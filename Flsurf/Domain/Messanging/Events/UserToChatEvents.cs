using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatBookmarked(Guid userId, Guid chatId) : BaseEvent
    {
        public Guid UserId { get; } = userId;
        public Guid ChatId { get; } = chatId; 
    }
}
