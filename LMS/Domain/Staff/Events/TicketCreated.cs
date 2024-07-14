using Flsurf.Domain.Staff.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketCreated(TicketEntity ticket) : BaseEvent
    {
        public TicketEntity Ticket { get; set; } = ticket;
    }
}
