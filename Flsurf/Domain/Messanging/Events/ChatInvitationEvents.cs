using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatInvitationUsed(ChatInvitationEntity invitation) : BaseEvent
    {
        public ChatInvitationEntity Invitation { get; set; } = invitation;
    }

    public class ChatInvitationCreated(ChatInvitationEntity invitation) : BaseEvent
    {
        public ChatInvitationEntity Invitation { get; set; } = invitation;
    }
}
