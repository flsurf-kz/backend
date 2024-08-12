using SpiceDb.Models;

namespace Flsurf.Application.Staff.Perms
{
    public class ZedTicket : ResourceReference
    {
        private ZedTicket(string ticketId) : base($"flsurf/ticket:{ticketId}") { }

        public static ZedTicket WithId(Guid ticketId) => new ZedTicket(ticketId.ToString()); 
    }
}
