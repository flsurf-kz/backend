using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;

namespace Flsurf.Domain.User.Events
{
    public class UserRoleUpdated(UserEntity user, UserRoles role) : DomainEvent
    {
        public UserEntity User { get; set; } = user;
        public UserRoles Role { get; set; } = role; 
    }
}
