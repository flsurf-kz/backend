using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.EventStore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public class EventDispatcher : IEventDispatcher
    {
        // Два словаря:
        // - Первый для доменных обработчиков
        private readonly Dictionary<Type, List<Type>> _domainSubscribers = new();
        // - Второй для интеграционных
        private readonly Dictionary<Type, List<Type>> _integrationSubscribers = new();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<Type> _ignoredTypes = new();

        public EventDispatcher(
            ILogger<EventDispatcher> logger,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
        }

        public void AddIgnoredTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                _logger.LogInformation("Added {TypeName} to ignore list", type.FullName);
                _ignoredTypes.Add(type);
            }
        }

        /// <summary>
        /// Регистрирует все классы, реализующие доменный или интеграционный интерфейс.
        /// </summary>
        public void RegisterEventSubscribers(Assembly assembly, IServiceScope scope)
        {
            var subscriberTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !_ignoredTypes.Contains(t))
                .Where(ImplementsAnyEventSubscriberInterface)
                .ToList();

            _logger.LogInformation("Registering event subscribers: {Types}",
                string.Join(", ", subscriberTypes.Select(t => t.FullName)));

            foreach (var subscriberType in subscriberTypes)
            {
                // Убедимся, что этот тип зарегистрирован в DI
                scope.ServiceProvider.GetRequiredService(subscriberType);

                // Доменные хендлеры
                var domainEventTypes = GetDomainEventTypesFromSubscriber(subscriberType);
                foreach (var domainEventType in domainEventTypes)
                {
                    if (!_domainSubscribers.ContainsKey(domainEventType))
                        _domainSubscribers[domainEventType] = new List<Type>();

                    if (!_domainSubscribers[domainEventType].Contains(subscriberType))
                    {
                        _domainSubscribers[domainEventType].Add(subscriberType);
                        _logger.LogInformation("Added domain subscriber {Subscriber} for event type {EventType}",
                            subscriberType.FullName, domainEventType.FullName);
                    }
                }

                // Интеграционные хендлеры
                var integrationEventTypes = GetIntegrationEventTypesFromSubscriber(subscriberType);
                foreach (var integrationEventType in integrationEventTypes)
                {
                    if (!_integrationSubscribers.ContainsKey(integrationEventType))
                        _integrationSubscribers[integrationEventType] = new List<Type>();

                    if (!_integrationSubscribers[integrationEventType].Contains(subscriberType))
                    {
                        _integrationSubscribers[integrationEventType].Add(subscriberType);
                        _logger.LogInformation("Added integration subscriber {Subscriber} for event type {EventType}",
                            subscriberType.FullName, integrationEventType.FullName);
                    }
                }
            }
        }

        // ============== Методы для диспатча ==============

        /// <summary>
        /// Обработать доменные события (все хендлеры, реализующие IDomainEventSubscriber<T>).
        /// Вызывается до коммита транзакции.
        /// </summary>
        public async Task DispatchDomainEventAsync<TEvent>(TEvent @event, IApplicationDbContext dbContext) where TEvent : BaseEvent
        {
            var eventType = @event.GetType();
            _logger.LogInformation("Dispatching DOMAIN event: {EventType}", eventType.Name);

            if (_domainSubscribers.ContainsKey(eventType))
            {
                foreach (var subscriberType in _domainSubscribers[eventType])
                {
                    using var scope = _scopeFactory.CreateScope();
                    var subscriber = scope.ServiceProvider.GetRequiredService(subscriberType);
                    // Вызываем dynamic-инвокацию
                    dynamic dynamicSubscriber = Convert.ChangeType(subscriber, subscriberType);
                    await dynamicSubscriber.HandleEvent((dynamic)@event, dbContext);
                }
            }
        }

        /// <summary>
        /// Обработать интеграционные события (все хендлеры, реализующие IIntegrationEventSubscriber<T>).
        /// Вызывается после коммита транзакции.
        /// </summary>
        public async Task DispatchIntegrationEventAsync<TEvent>(TEvent @event) where TEvent : BaseEvent
        {
            var eventType = @event.GetType();
            _logger.LogInformation("Dispatching INTEGRATION event: {EventType}", eventType.Name);

            if (_integrationSubscribers.ContainsKey(eventType))
            {
                foreach (var subscriberType in _integrationSubscribers[eventType])
                {
                    using var scope = _scopeFactory.CreateScope();
                    var subscriber = scope.ServiceProvider.GetRequiredService(subscriberType);
                    dynamic dynamicSubscriber = Convert.ChangeType(subscriber, subscriberType);
                    // Здесь dbContext обычно не нужен, поэтому null
                    await dynamicSubscriber.HandleEvent((dynamic)@event);
                }
            }
        }

        // ========== Вспомогательные методы ===========

        private bool ImplementsAnyEventSubscriberInterface(Type type)
        {
            return type.GetInterfaces().Any(i =>
                i.IsGenericType && (
                    i.GetGenericTypeDefinition() == typeof(IDomainEventSubscriber<>) ||
                    i.GetGenericTypeDefinition() == typeof(IIntegrationEventSubscriber<>)
                ));
        }

        private IEnumerable<Type> GetDomainEventTypesFromSubscriber(Type subscriberType)
        {
            // Ищем все интерфейсы вида: IDomainEventSubscriber<TEvent>
            return subscriberType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventSubscriber<>))
                .Select(i => i.GetGenericArguments()[0]);
        }

        private IEnumerable<Type> GetIntegrationEventTypesFromSubscriber(Type subscriberType)
        {
            // Ищем все интерфейсы вида: IIntegrationEventSubscriber<TEvent>
            return subscriberType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventSubscriber<>))
                .Select(i => i.GetGenericArguments()[0]);
        }
    }

}
