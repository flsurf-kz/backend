using Flsurf.Domain.Messanging.Entities;
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
}
