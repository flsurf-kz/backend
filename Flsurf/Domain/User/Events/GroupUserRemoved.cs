using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class GroupUserRemoved(Guid groupId, Guid userId) : BaseEvent
    {
        public Guid GroupId { get; } = groupId;
        public Guid UserId { get; } = userId;
    }
}
