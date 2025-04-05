using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.Queries
{
    public class GetNotificationsQuery : BaseQuery
    {
        public Guid? UserId { get; set; }

        public UserRoles? Role { get; set; }

        public int Start { get; set; }

        public int Ends { get; set; }
    }

    public class GetNotificationsHandler : IQueryHandler<GetNotificationsQuery, ICollection<NotificationEntity>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetNotificationsHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<ICollection<NotificationEntity>> Handle(GetNotificationsQuery query)
        {
            // Начинаем построение запроса по уведомлениям
            var notificationsQuery = _context.Notifications.AsQueryable();

            // Если указан конкретный пользователь – проверяем, что он совпадает с текущим
            if (query.UserId != null)
            {
                var currentUser = await _permService.GetCurrentUser();
                if (query.UserId != currentUser.Id)
                    throw new AccessDenied("You are not same user!");

                notificationsQuery = notificationsQuery.Where(x => x.ToUserId == query.UserId);
            }

            // Если фильтр по роли задан, проверяем права и фильтруем через join с пользователями
            if (query.Role != null)
            {
                var currentUser = await _permService.GetCurrentUser();
                await _permService.EnforceCheckPermission(
                    ZedUser.WithId(currentUser.Id).CanReadWarnings()
                );

                notificationsQuery = notificationsQuery
                    .Join(
                        _context.Users,
                        notification => notification.ToUserId,
                        user => user.Id,
                        (notification, user) => new { Notification = notification, User = user }
                    )
                    .Where(x => x.User.Role == query.Role)
                    .Select(x => x.Notification);
            }

            // Применяем пагинацию (предполагается, что расширение Paginate определено)
            notificationsQuery = notificationsQuery.Paginate(query.Start, query.Ends);

            // Выполняем запрос без отслеживания изменений
            return await notificationsQuery.AsNoTracking().ToListAsync();
        }
    }
}
