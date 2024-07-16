using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Staff.Events
{
    public class TicketAccepted(UserEntity byUser, TicketEntity ticket) : BaseEvent
    {
        public UserEntity ByUser { get; } = byUser;
        public TicketEntity Ticket { get; } = ticket;
    }
}
