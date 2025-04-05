using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class ResetPasswordCommand : BaseCommand
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        [EmailAddress(ErrorMessage = "No email")]
        public string Email { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
    }

    public class ResetPasswordHandler(
            IApplicationDbContext _context,
            IMemoryCache _cacheService,
            PasswordService _passwordService) : ICommandHandler<ResetPasswordCommand>
    {

        public async Task<CommandResult> Handle(ResetPasswordCommand command)
        {
            // Ищем пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email);
            if (user == null)
            {
                return CommandResult.NotFound($"Пользователь с email {command.Email} не найден.", command.Email);
            }

            // Получаем код из кэша по ключу, составленному из email и переданного кода
            var rawCode = _cacheService.Get(user.Email + command.Code);
            if (rawCode == null)
            {
                return CommandResult.BadRequest("Код сброса не найден или истек.");
            }

            try
            {
                var code = Convert.ToInt32(rawCode);
            }
            catch (Exception ex)
            {
                return CommandResult.UnprocessableEntity($"Ошибка при обработке кода: {ex.Message}");
            }

            // Обновляем пароль пользователя
            user.UpdatePassword(command.NewPassword, _passwordService);
            await _context.SaveChangesAsync();

            return CommandResult.Success();
        }
    }

}
