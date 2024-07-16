using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.User.Events
{
    public class UserCreated(UserEntity User) : BaseEvent
    {
        public UserEntity User { get; set; } = User;
    }
}
