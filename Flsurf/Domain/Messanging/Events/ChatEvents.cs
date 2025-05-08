namespace Flsurf.Domain.Messanging.Events
{
    public class ChatCreated(Guid chatId) : BaseEvent
    {
        public Guid ChatId { get; } = chatId;
    }

    public class ChatArchived(Guid chatId) : BaseEvent
    {
        public Guid ChatId { get; } = chatId;
    }

    public class ChatAddedParticipant(Guid chatId) : BaseEvent
    {
        public Guid ChatId { get; } = chatId;
    }

    public class ChatRemovedParticipant(Guid chatId, Guid userId, Guid kickedById) : BaseEvent
    {
        public Guid ChatId { get; } = chatId;
        public Guid UserId { get; } = userId;
        public Guid KickedById { get; } = kickedById;
    }

    public class ChatUpdated(Guid chatId) : BaseEvent
    {
        public Guid ChatId { get; } = chatId;
    }
}
