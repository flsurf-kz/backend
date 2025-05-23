﻿using Flsurf.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Flsurf.Infrastructure.EventStore
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreContext _context;
        private readonly IUser _user;
        private ILogger<EventStore> _logger;

        public EventStore(IEventStoreContext context, IUser user, ILogger<EventStore> logger)
        {
            _context = context;
            _user = user;
            _logger = logger;
        }

        public async Task StoreEvent<TEvent>(TEvent @event, bool isIntegrationEvent) where TEvent : BaseEvent
        {
            var jsonOptions = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            _logger.LogInformation("saving typeof: {tevent}, real type: {typeofType}, event itself {@event}", @event.GetType().Name, @event.GetType().FullName, @event); 

            var storedEvent = new StoredEvent
            {
                EventId = Guid.NewGuid(),
                ByUserId = _user.Id,
                EventType = @event.GetType().FullName ?? @event.GetType().Name, // можно FullName для точного соответствия
                Data = JsonConvert.SerializeObject(@event, jsonOptions),
                IsIntegrationEvent = isIntegrationEvent
            };

            await _context.Events.AddAsync(storedEvent);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<StoredEvent>> GetEvents(BaseEvent _eventType, DateTime? from = null, DateTime? to = null)
        {
            var eventType = _eventType.GetType().Name;
            IQueryable<StoredEvent> query = _context.Events.Where(e => e.EventType == eventType);

            if (from != null)
            {
                query = query.Where(e => e.Timestamp >= from);
            }

            if (to != null)
            {
                query = query.Where(e => e.Timestamp <= to);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<StoredEvent>> GetAllEvents(DateTime? from = null, DateTime? to = null)
        {
            IQueryable<StoredEvent> query = _context.Events;

            if (from != null)
            {
                query = query.Where(e => e.Timestamp >= from);
            }

            if (to != null)
            {
                query = query.Where(e => e.Timestamp <= to);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<StoredEvent>> GetEventsByAggregateId(BaseEvent _eventType, string aggregateId)
        {
            var eventType = _eventType.GetType().Name;
            return await _context.Events
                .Where(e => e.EventType == eventType && e.Data.Contains($"\"AggregateId\": \"{aggregateId}\""))
                .ToListAsync();
        }
    }
}
