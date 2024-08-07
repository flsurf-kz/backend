﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.EventStore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, List<Type>> eventListeners = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<Type> ignoredTypes = new HashSet<Type>(); // Новое поле для игнорируемых типов

        public EventDispatcher(
            ILogger<EventDispatcher> logger, IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public void AddIgnoredTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                _logger.LogInformation($"Added {type.FullName} into ignore list");
                ignoredTypes.Add(type);
            }
        }

        public void RegisterEventSubscribers(Assembly assembly, IServiceScope scope)
        {
            var eventSubscriberTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && ImplementsEventSubscriberInterface(type));
            _logger.LogInformation(
                $"Started to registering event subscribers, types: {string.Join(", ", eventSubscriberTypes.Select(t => t.FullName))}");
            foreach (var eventSubscriberType in eventSubscriberTypes)
            {
                if (ignoredTypes.Contains(eventSubscriberType)) // Проверка на игнорируемые типы
                {
                    _logger.LogInformation($"Skipping registration of ignored type {eventSubscriberType.FullName}");
                    continue;
                }
                // verfication! 
                scope.ServiceProvider.GetRequiredService(eventSubscriberType);
                var eventTypes = GetEventTypes(eventSubscriberType);
                foreach (var eventType in eventTypes)
                {
                    if (!eventListeners.ContainsKey(eventType))
                    {
                        eventListeners[eventType] = new List<Type>();
                    }

                    if (!eventListeners[eventType].Contains(eventSubscriberType))
                    {
                        eventListeners[eventType].Add(eventSubscriberType);
                        _logger.LogInformation($"Added event subscriber {eventSubscriberType.FullName} for event type {eventType.FullName}");
                    }
                }
            }
        }

        public void RegisterEventSubscriber<TEvent>(IEventSubscriber<TEvent> eventSubscriber) where TEvent : BaseEvent
        {
            var eventTypes = GetEventTypes(typeof(TEvent));

            foreach (var eventType in eventTypes)
            {
                if (!eventListeners.ContainsKey(eventType))
                {
                    eventListeners[eventType] = new List<Type>();
                }

                var eventSubscriberType = typeof(IEventSubscriber<>).MakeGenericType(typeof(TEvent));
                if (!eventListeners[eventType].Contains(eventSubscriberType))
                {
                    eventListeners[eventType].Add(eventSubscriberType);
                }
            }
        }

        public void AddListener<TEvent>(Action<TEvent> listener) where TEvent : BaseEvent
        {
            var eventType = typeof(TEvent);
            if (!eventListeners.ContainsKey(eventType))
            {
                eventListeners[eventType] = new List<Type>();
            }

            var listenerType = listener.GetType();
            if (!eventListeners[eventType].Contains(listenerType))
            {
                eventListeners[eventType].Add(listenerType);
            }
        }

        public void RemoveListener<TEvent>(Action<TEvent> listener) where TEvent : BaseEvent
        {
            var eventType = typeof(TEvent);
            if (eventListeners.ContainsKey(eventType))
            {
                var listenerType = listener.GetType();
                eventListeners[eventType].Remove(listenerType);
            }
        }

        public async Task Dispatch<TEvent>(TEvent @event, IApplicationDbContext dbContext) where TEvent : BaseEvent
        {
            _logger.LogInformation($"Dispatching event: {@event}, its type: {@event.GetType()}");

            var eventType = @event.GetType();

            if (eventListeners.ContainsKey(eventType))
            {
                foreach (var eventSubscriberType in eventListeners[eventType])
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var eventSubscriber = scope.ServiceProvider.GetRequiredService(eventSubscriberType);
                        dynamic dynamicSubscriber = Convert.ChangeType(eventSubscriber, eventSubscriberType);

                        await dynamicSubscriber.HandleEvent((dynamic)@event, dbContext);

                        // only after succesful completion, event could be added to eventStore. 
                        var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();

                        await eventStore.StoreEvent(@event);
                    }
                }
            }
        }

        private IEnumerable<Type> GetEventTypes(Type eventSubscriberType)
        {
            return eventSubscriberType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>))
                .Select(i => i.GetGenericArguments()[0]);
        }

        private bool ImplementsEventSubscriberInterface(Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSubscriber<>));
        }
    }
}
