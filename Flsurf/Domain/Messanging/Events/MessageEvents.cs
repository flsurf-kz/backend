using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Domain.Messanging.Events
{
    public class MessageCreated(Guid messageId) : BaseEvent
    {
        public Guid MessageId { get; } = messageId;
    }

    public class MessagePinned(Guid messageId) : BaseEvent
    {
        public Guid MessageId { get; } = messageId;
    }

    public class MessageUnpinned(Guid messageId) : BaseEvent
    {
        public Guid MessageId { get; } = messageId;
    }
}
