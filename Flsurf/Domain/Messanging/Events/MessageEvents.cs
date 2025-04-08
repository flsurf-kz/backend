using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class MessageCreated(MessageEntity message) : BaseEvent
    {
        public Guid MessageId { get; } = message.Id;
    }

    public class MessagePinned(MessageEntity message) : BaseEvent
    {
        public Guid MessageId { get; } = message.Id;
    }

    public class MessageUnpinned(MessageEntity message) : BaseEvent
    {
        public Guid MessageId { get; } = message.Id;
    }
}
