using SpiceDb.Models;

namespace Flsurf.Application.User.Permissions
{
    public class ZedWarning : ResourceReference
    {
        private ZedWarning(string notificationId) : base($"flsurf/warning:{notificationId}") { }

        public static ZedWarning WithId(Guid userId) => new ZedWarning(userId.ToString());

        public static ZedWarning WithWildcard() => new ZedWarning("*"); 
    }
}
