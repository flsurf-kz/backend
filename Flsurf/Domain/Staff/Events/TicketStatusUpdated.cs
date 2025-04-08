using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.Staff.Enums;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketStatusUpdated(TicketEntity ticket, TicketStatus status) : BaseEvent
    {
        public Guid TicketId { get; } = ticket.Id;
        public TicketStatus Status { get; } = status; 
    }
}
