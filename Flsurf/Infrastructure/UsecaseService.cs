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
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsInterface &&
                    IsBaseUseCase(type) &&
                    !ignoredTypes.Contains(type))
                .ToList();

            logger.LogInformation($"Найдено обработчиков: {useCaseTypes.Count}");

            foreach (var useCaseType in useCaseTypes)
            {
                services.AddScoped(useCaseType);
                logger.LogInformation($"Добавлен обработчик useCase-ов: {useCaseType.FullName}");
            }
        }

        private static bool IsBaseUseCase(Type type)
        {
            return type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(BaseUseCase<,>));
        }
    }
}
