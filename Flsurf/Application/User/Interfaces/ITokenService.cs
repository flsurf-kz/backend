using Flsurf.Domain.User.Entities;

namespace Flsurf.Application.User.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Генерирует токен (ключ) для указанного пользователя и сохраняет AuthenticationTicket в TicketStore.
        /// </summary>
        /// <param name="user">Пользователь, для которого создаем токен.</param>
        /// <returns>Строковое представление токена — ключ к записи в базе/кэше.</returns>
        Task<string> GenerateTokenAsync(UserEntity user);
    }
}
