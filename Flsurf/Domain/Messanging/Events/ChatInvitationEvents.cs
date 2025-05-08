using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class ChatInvitationUsed(Guid invitationId) : BaseEvent
    {
        public Guid InvitationId { get; } = invitationId;
    }

    public class ChatInvitationCreated(Guid invitationId) : BaseEvent
    {
        public Guid InvitationId { get; } = invitationId;
    }
}
