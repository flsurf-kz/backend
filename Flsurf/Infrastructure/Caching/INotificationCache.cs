using Flsurf.Domain.User.Entities;

namespace Flsurf.Infrastructure.Caching
{
    public interface INotificationCache
    {
        Task<ICollection<NotificationEntity>> GetNewNotifications(Guid userId);
        Task AddNotification(Guid userId, NotificationEntity notification);
    }
}
