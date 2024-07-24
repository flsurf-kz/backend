using SpiceDb.Models;

namespace Flsurf.Application.User.Permissions
{
    // Spec: read, create, update, delete 
    public class ZedUser : ResourceReference
    {
        private ZedUser(Guid userId) : base($"flsurf/user:{userId}") {  }
        
        public static ZedUser WithId(Guid userId) => new(userId);

        public Permission CanCreateNotification(ZedNotification notification) => new(this, "create", notification); 

        public Permission CanCreateWarnings() => new(this, "create", ZedWarning.WithWildcard());

        public Permission CanReadWarning(ZedWarning warning) => new(this, "read", warning);

        public Permission CanReadWarnings() => new(this, "", ZedWarning.WithWildcard()); 

        public Permission CanDeleteWarning(ZedWarning warning) => new(this, "delete", warning);

        public Permission CanUpdateRole(ZedUser user) => new(this, "can_update_role", user); 
    }
}
