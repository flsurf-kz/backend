using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class GetNotifications : BaseUseCase<GetNotificationsDto, ICollection<NotificationEntity>>
    {
        private IApplicationDbContext _context;
        private IPermissionService _permService;

        public GetNotifications(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<NotificationEntity>> Execute(GetNotificationsDto dto)
        {
            var query = _context.Notifications.AsQueryable();

            if (dto.UserId != null)
            {
                if (dto.UserId != (await _permService.GetCurrentUser()).Id)
                    throw new AccessDenied("You are not same user!"); 

                query = query.Where(x => x.ToUserId == dto.UserId);
            }
            if (dto.Role != null)
            {
                await _permService.EnforceCheckPermission(
                    ZedUser
                        .WithId((await _permService.GetCurrentUser()).Id)
                        .CanReadWarnings()); 

                query = query
                    .Join(
                        _context.Users,
                        notification => notification.ToUserId,
                        user => user.Id,
                        (notification, user) => new { Notification = notification, User = user }
                    )
                    .Where(x => x.User.Role == dto.Role)
                    .Select(x => x.Notification);
            }

            query = query.Paginate(dto.Start, dto.Ends);

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
