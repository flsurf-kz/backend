using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Presentation.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.EventHandlers
{
    public class MessageCreatedIntegrationHandler
        : IIntegrationEventSubscriber<MessageCreated>
    {
        private readonly IHubContext<GeneralHub> _hub;
        private readonly ILogger<MessageCreatedIntegrationHandler> _log;
        private readonly IApplicationDbContext _context; 

        public MessageCreatedIntegrationHandler(
            IHubContext<GeneralHub> hub,
            ILogger<MessageCreatedIntegrationHandler> log, 
            IApplicationDbContext context)
        {
            _hub = hub;
            _log = log;
            _context = context; 
        }

        public async Task HandleEvent(MessageCreated evt)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == evt.MessageId);

            if (message == null)
            {
                _log.LogError("Message with {Id} does not exists, why? i dont know! ", evt.MessageId);
                return; 
            }

            var dto = new
            {
                evt.MessageId,
                message.ChatId,
                message.SenderId,
                message.Text,
                message.CreatedAt, 
            };

            // шлём в группу чата
            await _hub.Clients
                      .Group($"chat:{message.ChatId}")
                      .SendAsync("ReceiveMessage", dto);

            _log.LogInformation("Pushed message {Id} to chat:{ChatId}", evt.MessageId, message.ChatId);
        }
    }

}
