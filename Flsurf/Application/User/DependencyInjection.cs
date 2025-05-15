using Flsurf.Application.Common.Events;
using Flsurf.Application.User.DomainHandlers;
using Flsurf.Application.User.EventHandlers;
using Flsurf.Application.User.Interfaces;
using Flsurf.Domain.User.Events;

namespace Flsurf.Application.User
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<UserCreatedDomainHandler>();
            services.AddScoped<UserCreatedIntegrationHandler>();
            services.AddScoped<NotificationCreatedIntegrationHandler>(); 

            return services;
        }
    }
}
