using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.User.Dto;
using Flsurf.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Flsurf.Application.User.UseCases
{
    public class ResetPassword : BaseUseCase<ResetPasswordDto, bool>
    {
        private IApplicationDbContext _context;
        private IMemoryCache _cacheService;
        private PasswordService _passwordService;

        public ResetPassword(
            IApplicationDbContext dbContext,
            IMemoryCache cacheService,
            PasswordService passwordService)
        {
            _context = dbContext;
            _cacheService = cacheService;
            _passwordService = passwordService;
        }

        public async Task<bool> Execute(ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            Guard.Against.NotFound(dto.Email, user);

            var rawCode = _cacheService.Get(user.Email + dto.Code);

            Guard.Against.NotFound(user.Email, rawCode);

            try
            {
                var code = (int)rawCode;
            }
            catch (InvalidCastException)
            {
                throw new Exception($"Что то пошло очень не так, code: {rawCode}");
            }

            user.UpdatePassword(dto.NewPassword, _passwordService);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
