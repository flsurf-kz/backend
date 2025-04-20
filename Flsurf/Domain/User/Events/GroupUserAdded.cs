using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class GroupUserAdded(Guid groupId, Guid user) : BaseEvent
    {
        public Guid GroupId { get; } = groupId;
        public Guid UserId { get; } = user;
    }
}
