namespace Flsurf.Domain.User.Events
{
    public class NotificationCreated(Guid notificationId) : BaseEvent
    {
        public Guid NotificationId { get; set; } = notificationId; 
    }
}
