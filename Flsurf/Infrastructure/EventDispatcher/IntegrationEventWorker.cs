using Flsurf.Application.Common.Events;
using Flsurf.Infrastructure.EventStore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Flsurf.Infrastructure.EventDispatcher
{
    public class IntegrationEventWorker : BackgroundService
    {
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

                // Ждём 5 секунд
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ProcessIntegrationEventsAsync(CancellationToken stoppingToken)
        {
            // 1. Создаём scope (DI), получаем IEventStoreContext
            using var scope = _serviceScopeFactory.CreateScope();
            var eventStoreContext = scope.ServiceProvider.GetRequiredService<IEventStoreContext>();

            // Получаем все не обработанные интеграционные события
            var storedIntegrationEvents = await eventStoreContext.Events
                .Where(e => e.IsIntegrationEvent && !e.Processed) // Ваша логика
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
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, можно сделать Retries++
                    _logger.LogError(ex, "Ошибка при обработке события {EventId}", storedEvent.EventId);
                    SentrySdk.CaptureException(ex);
                }
            }

            // 5. Сохраняем изменения (отмечаем Processed = true)
            await eventStoreContext.SaveChangesAsync(stoppingToken);
        }
    }

}
