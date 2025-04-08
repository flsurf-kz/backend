using Flsurf.Domain.Messanging.Entities;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatCreated(ChatEntity chat) : BaseEvent
    {
        public Guid ChatId { get; } = chat.Id;
    }

    public class ChatArchived(ChatEntity chat) : BaseEvent
    {
        public Guid ChatId { get; } = chat.Id;
    }

    public class ChatAddedParticipant(ChatEntity chat) : BaseEvent
    {
        public Guid ChatId { get; } = chat.Id;
    }

    public class ChatRemovedParticipant(ChatEntity chat, UserEntity user, UserEntity kickedBy) : BaseEvent
    {
        public Guid ChatId { get; } = chat.Id;
        public Guid UserId { get; } = user.Id;
        public Guid KickedById { get; } = kickedBy.Id;
    }

    public class ChatUpdated(ChatEntity chat) : BaseEvent
    {
        public Guid ChatId { get; } = chat.Id;
    }
}
