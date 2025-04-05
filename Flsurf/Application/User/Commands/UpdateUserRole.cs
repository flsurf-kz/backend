using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class UpdateUserRoleCommand : BaseCommand
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public UserRoles Role { get; set; }
    }

    public class UpdateUserRoleHandler : ICommandHandler<UpdateUserRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public UpdateUserRoleHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(UpdateUserRoleCommand command)
        {
            // Находим пользователя, которого нужно обновить
            var user = await _context.Users
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (user == null)
            {
                return CommandResult.NotFound("User not found.", command.UserId);
            }

            // Дополнительная проверка (например, получаем текущего пользователя для проверки прав)
            var byUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (byUser == null)
            {
                return CommandResult.NotFound("User not found.", command.UserId);
            }

            // Проверяем права на обновление роли
            await _permService.EnforceCheckPermission(
                ZedUser.WithId(byUser.Id).CanUpdateRole(ZedUser.WithId(user.Id))
            );

            user.SetRole(command.Role);
            await _context.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
