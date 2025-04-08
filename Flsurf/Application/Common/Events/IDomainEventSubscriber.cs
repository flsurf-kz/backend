using Flsurf.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Common.Events
{
    public interface IDomainEventSubscriber<in TEvent> where TEvent : BaseEvent
    {
        /// <summary>
        /// Обработка события внутри транзакции (до коммита).
        /// </summary>
        Task HandleEvent(TEvent @event, IApplicationDbContext dbContext);
    }

    public interface IIntegrationEventSubscriber<in TEvent> where TEvent : BaseEvent
    {
        /// <summary>
        /// Обработка события после коммита (вне транзакции).
        /// </summary>
        Task HandleEvent(TEvent @event);
    }
}
