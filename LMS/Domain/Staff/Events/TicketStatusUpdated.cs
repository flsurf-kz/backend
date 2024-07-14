using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.Staff.Enums;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketStatusUpdated(TicketEntity ticket, TicketStatus status) : BaseEvent
    {
        public TicketEntity Ticket { get; set; } = ticket;
        public TicketStatus Status { get; set; } = status;
    }
}
