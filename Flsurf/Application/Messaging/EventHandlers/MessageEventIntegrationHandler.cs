using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Events;
using Flsurf.Presentation.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Messaging.EventHandlers
{
    public sealed class MessageCreatedIntegrationHandler
        : IIntegrationEventSubscriber<MessageCreated>
    {
        private readonly IHubContext<GeneralHub> _hub;
        private readonly ILogger<MessageCreatedIntegrationHandler> _log;
        private readonly IApplicationDbContext _db;

        public MessageCreatedIntegrationHandler(
            IHubContext<GeneralHub> hub,
            ILogger<MessageCreatedIntegrationHandler> log,
            IApplicationDbContext db)
        {
            _hub = hub;
            _log = log;
            _db = db;
        }

        public async Task HandleEvent(MessageCreated evt)
        {
            var dto = await _db.Messages
                .Where(m => m.Id == evt.MessageId)
                .Select(m => new
                {
                    m.Id,
                    m.ChatId,
                    Payload = new MessageContract
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        Text = m.Text,
                        CreatedAt = m.CreatedAt,
                        Files = m.Files             // → ICollection<FileEntity>
                            .Select(f => new FileAttachmentDto
                            {
                                Id = f.Id,
                                FileName = f.FileName,
                                MimeType = f.MimeType,
                                Size = f.Size,
                                FilePath = f.FilePath, 
                            })
                            .ToList()
                    }
                })
                .SingleOrDefaultAsync();

            if (dto is null)
            {
                _log.LogWarning("Message {Id} not found – event skipped.", evt.MessageId);
                return;
            }

            await _hub.Clients.Group($"chat:{dto.ChatId}")
                              .SendAsync("ReceiveMessage", dto.ChatId, dto.Payload);
        }
    }

}
