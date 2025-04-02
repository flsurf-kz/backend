using Flsurf.Application.Common.cqrs;
using System.Reflection;

namespace Flsurf.Infrastructure
{
    public static class CQRSService
    {
        public static void AddCommandAndQueryHandlers(this IServiceCollection services, Assembly assembly, params Type[] ignoredTypes)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger("CqrsRegister");

            var commandQueriesHandlerTypes = assembly.GetTypes()
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsInterface &&
                    IsCommandOrQueryHandler(type) &&
                    !ignoredTypes.Contains(type))
                .ToList();

            logger.LogInformation($"[CQRS] Найдено обработчиков: {commandQueriesHandlerTypes.Count}");

            foreach (var handler in commandQueriesHandlerTypes)
            {
                services.AddScoped(handler);
                logger.LogInformation($"[CQRS] Добавлен обработчик: {handler.FullName}");
            }
        }

        private static bool IsCommandOrQueryHandler(Type type)
        {
            return type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                (
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                ));
        }
    }
}
