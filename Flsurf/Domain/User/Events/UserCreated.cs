using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class UserCreated(UserEntity user) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
    }
}
