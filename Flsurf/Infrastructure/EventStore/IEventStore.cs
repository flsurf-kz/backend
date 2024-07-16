using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Infrastructure.EventStore
{
    public interface IEventStore
    {
        Task StoreEvent<TEvent>(TEvent @event) where TEvent : BaseEvent;
        Task<IEnumerable<StoredEvent>> GetEvents(BaseEvent _eventType, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<StoredEvent>> GetAllEvents(DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<StoredEvent>> GetEventsByAggregateId(BaseEvent _eventType, string aggregateId);
    }
}
