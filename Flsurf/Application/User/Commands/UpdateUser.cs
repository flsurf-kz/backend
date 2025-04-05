using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.User.Dto;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class UpdateUserCommand : BaseCommand
    {
        [Required]
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public UserRoles? Role { get; set; }
        public CreateFileDto? Avatar { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? TelegramId { get; set; }
    }

    public class UpdateUserHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly PasswordService _passwordService;
        private readonly IFileService _fileService;

        public UpdateUserHandler(
            IApplicationDbContext context,
            IPermissionService permService,
            PasswordService passwordService,
            IFileService fileService)
        {
            _context = context;
            _permService = permService;
            _passwordService = passwordService;
            _fileService = fileService;
        }

        public async Task<CommandResult> Handle(UpdateUserCommand command)
        {
            // Находим пользователя по идентификатору
            var user = await _context.Users
                .IncludeStandard() // расширение для Include стандартных навигационных свойств
                .FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (user == null)
            {
                return CommandResult.NotFound("User does not exist.", command.UserId);
            }

            // Если требуется, можно получить текущего пользователя (например, для проверки прав)
            // Здесь предполагается, что "byUser" должен совпадать с найденным user.
            var byUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (byUser == null || byUser.Id != user.Id)
            {
                return CommandResult.Forbidden("You are not the user.");
            }

            // Обновляем поля, если они заданы
            if (!string.IsNullOrEmpty(command.Email))
            {
                user.Email = command.Email;
            }
            if (!string.IsNullOrEmpty(command.Name))
            {
                user.Name = command.Name;
            }
            if (!string.IsNullOrEmpty(command.Surname))
            {
                user.Surname = command.Surname;
            }
            if (!string.IsNullOrEmpty(command.TelegramId))
            {
                user.TelegramId = command.TelegramId;
            }
            if (!string.IsNullOrEmpty(command.NewPassword) && !string.IsNullOrEmpty(command.OldPassword))
            {
                try
                {
                    user.UpdatePassword(
                        oldPassword: command.OldPassword,
                        newPassword: command.NewPassword,
                        passwordService: _passwordService);
                }
                catch (Exception ex)
                {
                    return CommandResult.BadRequest($"Password update failed: {ex.Message}");
                }
            }
            if (command.Avatar != null)
            {
                // Загружаем файл асинхронно через IFileService
                var uploadedAvatar = await _fileService.UploadFile().Execute(command.Avatar);
                user.Avatar = uploadedAvatar;
            }
            // Дополнительно можно обработать поля Description, Blocked, Role и т.д.

            await _context.SaveChangesAsync();
            return CommandResult.Success();
        }
    }
}
