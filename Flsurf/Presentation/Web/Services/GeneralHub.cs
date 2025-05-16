using Microsoft.AspNetCore.SignalR;

namespace Flsurf.Presentation.Web.Services
{
    public class GeneralHub : Hub
    {
        // уже были методы для уведомлений…
        public Task JoinUser(Guid userId)
         => Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        public Task Subscribe(string channel)
         => Groups.AddToGroupAsync(Context.ConnectionId, channel);

        public Task Unsubscribe(string channel)
         => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);

        // добавляем методы для чата:
        public Task JoinChat(Guid chatId)
         => Groups.AddToGroupAsync(Context.ConnectionId, $"chat:{chatId}");

        public Task LeaveChat(Guid chatId)
         => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat:{chatId}");
    }
}