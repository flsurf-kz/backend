using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.UseCases
{
    public class CreateNotification : BaseUseCase<CreateNotificationDto, NotificationCreatedDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;


        public CreateNotification(IApplicationDbContext dbContext, IAccessPolicy accessPolicy)
        {
            _context = dbContext;
            _accessPolicy = accessPolicy;
        }

        public async Task<NotificationCreatedDto> Execute(CreateNotificationDto dto)
        {
            List<UserEntity> users = new List<UserEntity>();

            if (dto.Role != null)
            {
                if (dto.Role == UserRoles.User)
                {
                    throw new AccessDenied("Impossible to ping that many users");
                }

                users = await _context.Users
                    .AsNoTracking()
                    .Where(x => 
                        x.Roles.Any(x => x.Role == dto.Role))
                    .ToListAsync();
            }
            else if (dto.UserId != null)
            {
                await _accessPolicy.EnforceIsAllowed(PermissionEnum.write, _context.Notifications.EntityType); 

                users = await _context.Users.AsNoTracking().Where(x => x.Id == dto.UserId).ToListAsync();
            }

            var countUsers = users.Count;

            foreach (var user in users)
            {
                var notification = NotificationEntity.CreateFromSystem(
                    dto.Title, dto.Text, user.Id, dto.Data);

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return new()
            {
                UserIds = users.Select(x => x.Id).ToList(),
                SentToUsers = countUsers,
            };
        }
    }
}
