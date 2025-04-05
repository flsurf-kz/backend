using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.User.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{

    public class WarnUserCommand : BaseCommand
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Reason { get; set; } = null!;
    }

    public class WarnUserHandler : ICommandHandler<WarnUserCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public WarnUserHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(WarnUserCommand command)
        {
            // Ищем пользователя, которому нужно выдать предупреждение
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (user == null)
            {
                return CommandResult.NotFound($"Пользователь с id {command.UserId} не найден.", command.UserId);
            }

            // Получаем текущего пользователя для проверки прав
            var currentUser = await _permService.GetCurrentUser();

            // Проверяем, имеет ли текущий пользователь право создавать предупреждения
            await _permService.EnforceCheckPermission(
                ZedUser.WithId(currentUser.Id).CanCreateWarnings()
            );

            // Вызываем метод предупреждения пользователя
            user.Warn(command.Reason, currentUser);
            await _context.SaveChangesAsync();

            // Возвращаем успех, можно вернуть изменённого пользователя
            return CommandResult.Success();
        }
    }
}
