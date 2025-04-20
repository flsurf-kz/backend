using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class UserBlocked(Guid user) : DomainEvent
    {
        public Guid UserId { get; } = user;
    }
}
