using SpiceDb.Models;

namespace Flsurf.Application.User.Permissions
{
    // Spec: read, create, update, delete 
    public class ZedUser : ResourceReference
    {
        private ZedUser(Guid userId) : base($"flsurf/user:{userId}") {  }
        
        public static ZedUser WithId(Guid userId) => new(userId);

        public Permission CanCreateNotifications() => new(ZedNotification.WithWildcard(), "create", this); 

        public Permission CanCreateWarnings() => new(this, "create", ZedWarning.WithWildcard());

        public Permission CanReadWarning(ZedWarning warning) => new(warning, "read", this);

        public Permission CanReadWarnings() => new(this, "", ZedWarning.WithWildcard()); 

        public Permission CanDeleteWarning(ZedWarning warning) => new(warning, "delete", this);

        public Permission CanUpdateRole(ZedUser user) => new(user, "can_update_role", this); 
    }
}
