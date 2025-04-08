using Flsurf.Domain.Staff.Entities;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketCreated(TicketEntity ticket) : BaseEvent
    {
        public Guid TicketId { get; } = ticket.Id; 
    }
}
