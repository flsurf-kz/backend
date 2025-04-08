using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class UserDeleted(UserEntity user, string reason) : BaseEvent
    {
        public Guid UserId { get; } = user.Id;
        public string Reason { get; } = reason;
    }
}
