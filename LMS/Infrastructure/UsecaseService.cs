using Flsurf.Application.Common.UseCases;
using System.Reflection;

namespace Flsurf.Infrastructure
{
    public static class UseCaseServiceRegistration
    {
        public static void AddUseCasesFromAssembly(this IServiceCollection services, Assembly assembly, params Type[] ignoredTypes)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger("UseCaseRegister");
            
            var useCaseTypes = assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && IsBaseUseCase(type) && !ignoredTypes.Contains(type))
                .ToList();

            foreach (var useCaseType in useCaseTypes)
            {
                services.AddScoped(useCaseType);

                logger.LogInformation($"Added use case service: {useCaseType.FullName}");
            }
        }

        private static bool IsBaseUseCase(Type type)
        {
            return type.BaseType != null && type.BaseType.IsGenericType &&
                   type.BaseType.GetGenericTypeDefinition() == typeof(BaseUseCase<,>);
        }
    }
}
