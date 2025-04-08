using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;

namespace Flsurf.Domain.User.Events
{
    public class UserRoleUpdated(UserEntity user, UserRoles role) : DomainEvent
    {
        public Guid UserId { get; } = user.Id;
        public UserRoles Role { get; } = role;
    }
}
