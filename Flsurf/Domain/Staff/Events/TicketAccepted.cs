using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketAccepted(UserEntity byUser, TicketEntity ticket) : BaseEvent
    {
        public Guid ByUserId { get; } = byUser.Id; 
        public Guid TicketId { get; } = ticket.Id; 
    }
}
