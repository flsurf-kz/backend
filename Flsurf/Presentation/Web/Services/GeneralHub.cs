using Flsurf.Application.Messaging.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Flsurf.Presentation.Web.Services
{
    [Authorize]                       // общая защита: в хаб попадают только залогиненные
    public sealed class GeneralHub : Hub
    {
        /* ---------------- helpers ---------------- */

        private static string UserGroup(Guid userId) => $"user:{userId}";
        private static string ChatGroup(Guid chatId) => $"chat:{chatId}";

        /* ---------------- life-cycle -------------- */

        public override async Task OnConnectedAsync()
        {
            // автоматически кладём соединение в личную группу пользователя
            Guid userId = GetUserId();
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            Guid userId = GetUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroup(userId));
            await base.OnDisconnectedAsync(ex);
        }

        /* --------------- публичные API ------------ */

        // личные уведомления
        public Task JoinUser(Guid userId)
            => Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

        public Task Subscribe(string channel)
            => Groups.AddToGroupAsync(Context.ConnectionId, channel);

        public Task Unsubscribe(string channel)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);

        /* ---------- чат-специфичные методы -------- */

        public Task JoinChatGroup(Guid chatId)
            => Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));

        public Task LeaveChatGroup(Guid chatId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroup(chatId));

        /// <summary>
        /// Принимает новое сообщение от клиента и рассылает всем участникам чата.
        /// </summary>
        public async Task SendChatMessage(Guid chatId, MessageContract payload)
        {
            // TODO: валидация и сохранение сообщения в БД
            await Clients.Group(ChatGroup(chatId))
                         .SendAsync("ReceiveMessage", chatId, payload);
        }

        /* ---------------- utils ------------------- */

        private Guid GetUserId()
        {
            // из JWT/кук, настроенных в ASP.NET Identity или собственном auth-мидлвари
            string? sub = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }
}