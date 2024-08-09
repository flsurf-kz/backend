using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Messaging.Dto;
using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Application.Messaging.UseCases
{
    public class SendMessage : BaseUseCase<SendMessageDto, MessageEntity>
    {
        public SendMessage() { }

        public async Task<MessageEntity> Execute(SendMessageDto dto)
        {
            return new();
        }
    }
}
