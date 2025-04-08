using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class UserBlocked(UserEntity user) : DomainEvent
    {
        public Guid UserId { get; } = user.Id;
    }
}
