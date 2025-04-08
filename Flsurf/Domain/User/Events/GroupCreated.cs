using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.User.Events
{
    public class GroupCreated(GroupEntity group) : DomainEvent
    {
        public Guid GroupId { get; } = group.Id;
    }
}
