using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.User.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Flsurf.Infrastructure.Data
{
    public class SecurityAnswerTokenService : ITokenService
    {
        private readonly ITicketStore _ticketStore;

        public SecurityAnswerTokenService(
            ITicketStore ticketStore)
        {
            _ticketStore = ticketStore;
        }

        public async Task<string> GenerateTokenAsync(UserEntity user)
        {
            // 1. Собираем Claims точно так же, как в вашем AuthController.CreatePrincipal(...)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Name,           user.Fullname),
                new Claim("IsExternalUser",          user.IsExternalUser.ToString()),
                new Claim("AvatarUrl",               user.Avatar?.FilePath ?? string.Empty),
                new Claim("UserType",                user.Type.ToString()),
                new Claim(ClaimTypes.Role,           user.Role.ToString()),
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            var principal = new ClaimsPrincipal(identity);

            // 2. Задаем время жизни токена — например, на 1 час
            var props = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            // 3. Упаковываем всё в AuthenticationTicket
            var ticket = new AuthenticationTicket(
                principal,
                props,
                CookieAuthenticationDefaults.AuthenticationScheme);

            // 4. Сохраняем в ваш TicketStore (кэш + БД) и получаем ключ
            var tokenKey = await _ticketStore.StoreAsync(ticket);

            // 5. Отдаем этот ключ клиенту
            return tokenKey;
        }
    }
}
