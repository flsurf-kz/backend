using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.User.Events;
using Flsurf.Presentation.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.EventHandlers
{
    public class NotificationCreatedIntegrationHandler
        : IIntegrationEventSubscriber<NotificationCreated>
    {
        private readonly IHubContext<GeneralHub> _hub;
        private readonly ILogger<NotificationCreatedIntegrationHandler> _logger;
        private readonly IApplicationDbContext _context;

        public NotificationCreatedIntegrationHandler(
            IHubContext<GeneralHub> hub,
            ILogger<NotificationCreatedIntegrationHandler> logger,
            IApplicationDbContext context)
        {
            _hub = hub;
            _logger = logger;
            _context = context;
        }

        public async Task HandleEvent(NotificationCreated evt)
        {
            // 1) Достать только что созданное уведомление из БД
            var notification = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == evt.NotificationId);

            if (notification == null)
            {
                _logger.LogWarning("Notification {Id} not found in database", evt.NotificationId);
                return;
            }

            // 2) Смапить на DTO
            var dto = new NotificationDto
            {
                NotificationId = notification.Id,
                Title = notification.Title,
                Text = notification.Text,
                FromUserId = notification.FromUserId,
                ToUserId = notification.ToUserId,
                Type = notification.Type,
                Data = notification.Data,
                Hidden = notification.Hidden,
                IconId = notification.Icon?.Id,
                CreatedAt = notification.CreatedAt,
                LastModifiedAt = notification.LastModifiedAt
            };

            // 3) Отправить персонально в группу user:{ToUserId}
            await _hub.Clients
                      .Group($"user:{dto.ToUserId}")
                      .SendAsync("ReceiveNotification", dto);

            // 4) Отправить в общий канал "notification"
            await _hub.Clients
                      .Group("notification")
                      .SendAsync("ReceiveNotification", dto);

            _logger.LogInformation("Notification {Id} pushed to WebSocket groups", dto.NotificationId);
        }
    }

}
