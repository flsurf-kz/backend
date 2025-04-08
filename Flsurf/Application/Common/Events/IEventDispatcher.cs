using Flsurf.Application.Common.Interfaces;
using Microsoft.OpenApi.Any;
using System.Reflection;

namespace Flsurf.Application.Common.Events
{
    public interface IEventDispatcher
    {
        void AddIgnoredTypes(IEnumerable<Type> types);
        void RegisterEventSubscribers(Assembly assembly, IServiceScope scope);

        /// <summary>
        /// Диспатчит событие для доменных обработчиков.
        /// Вызывается до транзакции (или внутри нее).
        /// </summary>
        Task DispatchDomainEventAsync<TEvent>(TEvent @event, IApplicationDbContext dbContext) where TEvent : BaseEvent;

        /// <summary>
        /// Диспатчит событие для интеграционных обработчиков.
        /// Вызывается после коммита.
        /// </summary>
        Task DispatchIntegrationEventAsync<TEvent>(TEvent @event) where TEvent : BaseEvent;
    }
}
