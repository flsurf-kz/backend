using SpiceDb.Models;

namespace Flsurf.Application.Messaging.Permissions
{
    public class ZedInvitation : ResourceReference
    {
        private ZedInvitation(Guid invitationId) : base($"flsurf/invitation:{invitationId}") { }

        public static ZedInvitation WithId(Guid invitationId) => new(invitationId);
    }
}
