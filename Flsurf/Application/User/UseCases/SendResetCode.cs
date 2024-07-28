using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Infrastructure.Adapters.Mailing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel;

namespace Flsurf.Application.User.UseCases
{
    public class SendResetCode : BaseUseCase<SendResetCodeDto, bool>
    {
        private IApplicationDbContext _context;
        private IEmailService _emailService;
        private IMemoryCache _cacheService;

        public SendResetCode(IEmailService emailService, IApplicationDbContext dbContext, IMemoryCache cacheService)
        {
            _context = dbContext;
            _emailService = emailService;
            _cacheService = cacheService;
        }

        public async Task<bool> Execute(SendResetCodeDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            Guard.Against.NotFound(dto.Email, user);

            Random rnd = new Random();

            var code = rnd.Next(100000);

            var newCode = _cacheService.Set(dto.Email + code, code);

            await _emailService.SendEmailAsync(
                dto.Email,
                "Возврат пароля для входа в ваш аккаунт",
                $"Ваш код для сброса пароля: {code}");

            return true;
        }
    }
}
