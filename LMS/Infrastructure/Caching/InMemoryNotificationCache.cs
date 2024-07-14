using Flsurf.Domain.User.Entities;

namespace Flsurf.Infrastructure.Caching
{
    public class InMemoryNotificationCache : INotificationCache
    {
        private readonly Dictionary<Guid, ICollection<NotificationEntity>> _notificationsDictionary = [];

        public Task<ICollection<NotificationEntity>> GetNewNotifications(Guid userId)
        {
            var messages = _notificationsDictionary
                .Where(x => x.Key == userId)
                .Select(x => x.Value)
                .FirstOrDefault();
            if (messages == null)
            {
                return Task.FromResult<ICollection<NotificationEntity>>([]);
            }
            _notificationsDictionary[userId].Clear();
            return Task.FromResult(messages);
        }

        public Task AddNotification(Guid userId, NotificationEntity message)
        {
            if (!_notificationsDictionary.ContainsKey(userId))
            {
                _notificationsDictionary[userId] = [];
            }
            _notificationsDictionary[userId].Add(message);
            return Task.CompletedTask;
        }
    }
}
