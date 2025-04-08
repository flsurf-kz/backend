using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatInvitationUsed(ChatInvitationEntity invitation) : BaseEvent
    {
        public Guid InvitationId { get; } = invitation.Id;
    }

    public class ChatInvitationCreated(ChatInvitationEntity invitation) : BaseEvent
    {
        public Guid InvitationId { get; } = invitation.Id;
    }
}
