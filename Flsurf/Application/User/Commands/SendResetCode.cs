using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Infrastructure.Adapters.Mailing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.User.Commands
{
    public class SendResetCodeCommand : BaseCommand
    {
        [Required]
        public string Email { get; set; } = null!;
    }

    public class SendResetCodeHandler(
            IEmailService _emailService,
            IApplicationDbContext _dbContext,
            IMemoryCache _cacheService) : ICommandHandler<SendResetCodeCommand>
    {
        public async Task<CommandResult> Handle(SendResetCodeCommand command)
        {
            // Ищем пользователя по email
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == command.Email);
            if (user == null)
            {
                return CommandResult.NotFound($"Пользователь с email {command.Email} не найден.", command.Email);
            }

            // Генерируем случайный код
            var rnd = new Random();
            var code = rnd.Next(100000); // Код в диапазоне от 0 до 99999
                                         // Формируем уникальный ключ для кэширования (можно добавить префикс, дату и т.п.)
            var cacheKey = command.Email + "_" + code;
            // Кэшируем код на 5 минут
            _cacheService.Set(cacheKey, code, TimeSpan.FromMinutes(5));

            // Отправляем email с кодом сброса пароля
            await _emailService.SendEmailAsync(
                command.Email,
                "Возврат пароля для входа в ваш аккаунт",
                $"Ваш код для сброса пароля: {code}");

            return CommandResult.Success();
        }
    }
}
