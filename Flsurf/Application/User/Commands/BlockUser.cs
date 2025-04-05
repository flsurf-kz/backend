using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
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
    public class BlockUserCommand : BaseCommand
    {
        [Required]
        public Guid UserId { get; set; }
        public bool? Blocked { get; set; } = true; 
    }

    public class BlockUserHandler(IApplicationDbContext _context, IPermissionService _permService) : ICommandHandler<BlockUserCommand>
    {
        public async Task<CommandResult> Handle(BlockUserCommand command)
        {
            // Получаем текущего пользователя через сервис прав
            var currentUser = await _permService.GetCurrentUser();

            // Проверяем, что текущий пользователь имеет право деактивировать (блокировать) целевого пользователя
            await _permService.CheckPermission(
                ZedUser
                    .WithId(currentUser.Id)
                    .CanDeactivateUser(ZedUser.WithId(command.UserId))
            );

            // Ищем пользователя по заданному идентификатору
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (user == null)
            {
                return CommandResult.NotFound("Пользователь не найден.", command.UserId);
            }

            // Выполняем блокировку пользователя
            user.Block(command.Blocked ?? true);

            await _context.SaveChangesAsync();

            // Можно вернуть, например, идентификатор заблокированного пользователя
            return CommandResult.Success(user.Id);
        }
    }
}
