using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatBookmarked(UserToChatEntity usertoChat) : BaseEvent {
        public UserToChatEntity UserToChat { get; set; } = usertoChat; 
    }
}
