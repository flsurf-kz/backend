using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.User.Events
{
    public class UserWarned(UserEntity user, string reason) : BaseEvent
    {
        public UserEntity User { get; set; } = user;
        public string Reason { get; set; } = reason;
    }
}
