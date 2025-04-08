using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Newtonsoft.Json;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public class LoggingHandler<Event> : IDomainEventSubscriber<Event> where Event : BaseEvent
    {
        private ILogger _logger;

        public LoggingHandler(ILogger<LoggingHandler<Event>> logger)
        {
            _logger = logger;
        }

        public Task HandleEvent(Event eventData, IApplicationDbContext context)
        {
            // Using Newtonsoft.Json because System.Text.Json
            // is a sad joke to be considered "Done"

            // The System.Text don't know how serialize a
            // object with inherited properties, I said is sad...
            // Yes! I tried: options = new JsonSerializerOptions { WriteIndented = true };
            var serializedData = JsonConvert.SerializeObject(eventData);

            _logger.LogInformation($"Event: {eventData.GetType()} has happend, data: {serializedData}");

            return Task.CompletedTask;
        }
    }
}
