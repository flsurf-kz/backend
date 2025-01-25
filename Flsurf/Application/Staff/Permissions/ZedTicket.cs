using Flsurf.Application.Messaging.Permissions;
using SpiceDb.Models;

namespace Flsurf.Application.Staff.Perms
{
    public class ZedTicket : ResourceReference
    {
        private ZedTicket(string notificationId) : base($"flsurf/ticket:{notificationId}") { }

        public static ZedTicket WithId(Guid notificationId) => new ZedTicket(notificationId.ToString());
        public static ZedTicket WithWildcard() => new ZedTicket("*"); 

        public Relationship Owner(ZedStaffUser user) => new(user, "owner", this);

        public Relationship Assigned(ZedStaffUser user) => new(user, "assigned", this); 
    }
}
