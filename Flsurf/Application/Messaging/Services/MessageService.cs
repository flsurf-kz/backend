using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Application.Messaging.UseCases;

namespace Flsurf.Application.Messaging.Services
{
    public class MessageService : IMessageService
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SendMessage Send()
        {
            return _serviceProvider.GetRequiredService<SendMessage>();
        }

        public GetMessagesList GetMessages()
        {
            return _serviceProvider.GetRequiredService<GetMessagesList>();
        }

        public DeleteMessage Delete()
        {
            return _serviceProvider.GetRequiredService<DeleteMessage>();
        }

        public UpdateMessage Update()
        {
            return _serviceProvider.GetRequiredService<UpdateMessage>();
        }

        public PinMessage Pin()
        {
            return _serviceProvider.GetRequiredService<PinMessage>();
        }
    }
}
