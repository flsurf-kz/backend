using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class GetNotifications : BaseUseCase<GetNotificationsDto, ICollection<NotificationEntity>>
    {
        private IApplicationDbContext _context;
        private IAccessPolicy _accessPolicy;

        public GetNotifications(IApplicationDbContext dbContext, IAccessPolicy accessPolicy)
        {
            _context = dbContext;
            _accessPolicy = accessPolicy;
        }

        public async Task<ICollection<NotificationEntity>> Execute(GetNotificationsDto dto)
        {
            var query = _context.Notifications.AsQueryable();

            if (dto.UserId != null)
            {
                await _accessPolicy.EnforceIsAllowed(
                    PermissionEnum.read, _context.Notifications.EntityType, (Guid)dto.UserId); 

                query = query.Where(x => x.ToUserId == dto.UserId);
            }
            if (dto.Role != null)
            {
                await _accessPolicy.Role(UserRoles.Admin);

                query = query
                    .Join(
                        _context.Users,
                        notification => notification.ToUserId,
                        user => user.Id,
                        (notification, user) => new { Notification = notification, User = user }
                    )
                    .Where(x => x.User.Roles.All(x => x.Role == dto.Role))
                    .Select(x => x.Notification);
            }

            query = query.Paginate(dto.Start, dto.Ends);

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
