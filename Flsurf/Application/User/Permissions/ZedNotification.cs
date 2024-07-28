using SpiceDb.Models;

namespace Flsurf.Application.User.Permissions
{
    public class ZedNotification : ResourceReference
    {
        private ZedNotification(string notificationId) : base($"flsurf/notification:{notificationId}") { }

        public static ZedNotification WithId(Guid notificationId) => new ZedNotification(notificationId.ToString());

        public static ZedNotification WithWildcard() => new ZedNotification("*");
    }
}
