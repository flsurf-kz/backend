using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class CreateNotificationCommand : BaseCommand
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Text { get; set; } = null!;
        [Required]
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        public Guid? UserId { get; set; }
        public UserRoles? Role { get; set; }
        public UserTypes? Type { get; set; }
        public bool Internal { get; internal set; }
    }

    public class CreateNotificationHandler : ICommandHandler<CreateNotificationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public CreateNotificationHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(CreateNotificationCommand command)
        {
            if (!command.Internal)
            {
                // Проверяем, имеет ли текущий пользователь право создавать предупреждения
                await _permService.CheckPermission(
                    ZedUser
                        .WithId((await _permService.GetCurrentUser()).Id)
                        .CanCreateWarnings()
                );
            }

            List<UserEntity> users = new List<UserEntity>();

            if (command.Role != null)
            {
                if (command.Role == UserRoles.User)
                {
                    // Если роль равна "User", считаем, что рассылка уведомлений невозможна
                    return CommandResult.Forbidden("Impossible to ping that many users");
                }

                users = await _context.Users
                    .Where(x => x.Role == command.Role)
                    .ToListAsync();
            }
            else if (command.UserId != null)
            {
                users = await _context.Users
                    .Where(x => x.Id == command.UserId)
                    .ToListAsync();
            }

            int countUsers = users.Count;

            // Создаем уведомление для каждого получателя
            foreach (var user in users)
            {
                var notification = NotificationEntity.CreateFromSystem(
                    command.Title,
                    command.Text,
                    user.Id,
                    command.Data,
                    null  // Если нужно, можно передать дополнительные данные
                );
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return CommandResult.Success(users.Select(x => x.Id).ToList());
        }
    }
}
