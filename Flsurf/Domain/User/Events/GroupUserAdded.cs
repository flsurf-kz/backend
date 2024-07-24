using Flsurf.Domain.User.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.User.Events
{
    public class GroupUserAdded(GroupEntity group, UserEntity user) : BaseEvent
    {
        public GroupEntity Group { get; set; } = group;
        public UserEntity User { get; set; } = user;
    }
}
