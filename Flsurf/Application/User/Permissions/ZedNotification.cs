using SpiceDb.Models;

namespace Flsurf.Application.User.Permissions
{
    public class ZedNotification : ResourceReference
    {
        private ZedNotification(Guid notificationId) : base($"flsurf/notification:{notificationId}") { }

        public static ZedNotification WithId(Guid userId) => new ZedNotification(userId);
    }
}
