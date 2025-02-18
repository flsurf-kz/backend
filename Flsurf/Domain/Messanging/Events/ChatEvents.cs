using Flsurf.Domain.Messanging.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatCreated(ChatEntity chat) : BaseEvent
    {
        public ChatEntity Chat { get; set; } = chat; 
    }

    public class ChatArchived(ChatEntity chat) : BaseEvent
    {
        public ChatEntity Chat { get; set; } = chat;
    }

    public class ChatAddedParticipant(ChatEntity chat) : BaseEvent
    {
        public ChatEntity Chat { get; set; } = chat; 
    }

    public class ChatRemovedParticipant(ChatEntity chat, UserEntity user, UserEntity kickedBy) : BaseEvent
    {
        public ChatEntity Chat { get; set; } = chat;
        public UserEntity User { get; set; } = user;
        public UserEntity KickedBy { get; set; } = kickedBy;
    }
    public class ChatUpdated(ChatEntity chat) : BaseEvent
    {
        public ChatEntity Chat { get; set; } = chat;
    }
}
