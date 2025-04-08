using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class GroupUserAdded(GroupEntity group, UserEntity user) : BaseEvent
    {
        public Guid GroupId { get; } = group.Id;
        public Guid UserId { get; } = user.Id;
    }
}
