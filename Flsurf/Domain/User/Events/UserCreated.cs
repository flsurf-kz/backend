using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class UserCreated(Guid userId) : BaseEvent
    {
        public Guid UserId { get; set; } = userId;
    }
}
