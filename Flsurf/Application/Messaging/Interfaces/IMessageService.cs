using Flsurf.Application.Messaging.UseCases;

namespace Flsurf.Application.Messaging.Interfaces
{
    public interface IMessageService
    {
        SendMessage Send();
        GetMessagesList GetMessages();
        DeleteMessage Delete();
        UpdateMessage Update();
        PinMessage Pin(); 
    }
}
