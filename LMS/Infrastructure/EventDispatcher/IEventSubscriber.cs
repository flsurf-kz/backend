using Flsurf.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public interface IEventSubscriber<TEvent> where TEvent : BaseEvent
    {
        public Task HandleEvent(TEvent @event, IApplicationDbContext dbContext);
    }
}
