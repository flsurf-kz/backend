using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.UseCases;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Flsurf.Infrastructure
{
    public static class CQRSService
    {
        public static void AddCommandAndQueryHandlers(this IServiceCollection services, Assembly assembly, params Type[] ignoredTypes)
        {
            var commandQueriesHandlerTypes = assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && IsCommandOrQueryHandler(type) && !ignoredTypes.Contains(type))
                .ToList();

            AddTypes(services, commandQueriesHandlerTypes); 
        }

        public static void AddTypes(this IServiceCollection services, List<Type> types)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger("UseCaseRegister");

            foreach (var handler in types)
            {
                services.AddScoped(handler);

                logger.LogInformation($"Added use case service: {handler.FullName}");
            }
        }

        private static bool IsCommandOrQueryHandler(Type type)
        {
            return type.BaseType != null && type.BaseType.IsGenericType &&
                   (
                       type.BaseType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                       type.BaseType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                   );
        }
    }
}
