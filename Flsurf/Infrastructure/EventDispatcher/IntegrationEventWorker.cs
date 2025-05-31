using Flsurf.Application.Common.Events;
using Flsurf.Infrastructure.EventStore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public class IntegrationEventWorker : BackgroundService
    {
        public static int MAX_FAILED_COUNTER = 3; 
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<IntegrationEventWorker> _logger;
        // Вам нужен EventDispatcher, чтобы вызывать хендлеры
        private readonly IEventDispatcher _eventDispatcher;

        public IntegrationEventWorker(
            IServiceScopeFactory serviceScopeFactory,
            IEventDispatcher eventDispatcher,
            ILogger<IntegrationEventWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _eventDispatcher = eventDispatcher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessIntegrationEventsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка в ProcessIntegrationEventsAsync");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
            }
        }

        private async Task ProcessIntegrationEventsAsync(CancellationToken stoppingToken)
        {
            // 1. Создаём scope (DI), получаем IEventStoreContext
            using var scope = _serviceScopeFactory.CreateScope();
            var eventStoreContext = scope.ServiceProvider.GetRequiredService<IEventStoreContext>();

            // Получаем все не обработанные интеграционные события
            var storedIntegrationEvents = await eventStoreContext.Events
                .Where(e => e.IsIntegrationEvent && !e.Processed && !e.ProcessError) // Ваша логика
                .OrderBy(e => e.Timestamp)
                .Take(50)
                .ToListAsync(stoppingToken);

            if (!storedIntegrationEvents.Any())
                return;

            foreach (var storedEvent in storedIntegrationEvents)
            {
                try
                {
                    // 2. Десериализовать событие
                    var eventType = Type.GetType(storedEvent.EventType);
                    if (eventType == null)
                    {
                        // Можно пометить как Processed или записать Error
                        storedEvent.Processed = true;
                        continue;
                    }

                    // Предположим, что вы сериализуете через Newtonsoft.Json
                    var @event = JsonConvert.DeserializeObject(storedEvent.Data, eventType) as BaseEvent;
                    if (@event == null)
                    {
                        storedEvent.Processed = true;
                        continue;
                    }

                    // 3. Вызываем интеграционные хендлеры
                    await _eventDispatcher.DispatchIntegrationEventAsync(@event);

                    // 4. Если всё ок, помечаем обработанным
                    storedEvent.Processed = true;
                    eventStoreContext.Events.Update(storedEvent);
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, можно сделать Retries++
                    _logger.LogError(ex, "Ошибка при обработке события {EventId}", storedEvent.EventId);
                    if (storedEvent.FailedCounter > MAX_FAILED_COUNTER) { 
                        // WHY? we need to filter out events that can not be executed 
                        var settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };
                        var log = ErrorLogEntry.FromException(ex);
                        var json = JsonConvert.SerializeObject(log, settings);
                        storedEvent.ProcessError = true;
                        storedEvent.Data = json;
                    } else
                    {
                        storedEvent.FailedCounter += 1; 
                    }
                    SentrySdk.CaptureException(ex);
                }
            }

            // 5. Сохраняем изменения (отмечаем Processed = true)
            await eventStoreContext.SaveChangesAsync(stoppingToken);
        }
    }

}
