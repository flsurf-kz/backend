using SpiceDb.Models;

namespace Flsurf.Application.Staff.Perms
{
    public class ZedTicket : ResourceReference
    {
        private ZedTicket(string notificationId) : base($"flsurf/ticket:{notificationId}") { }

        public static ZedTicket WithId(Guid notificationId) => new ZedTicket(notificationId.ToString()); 
    }
}
