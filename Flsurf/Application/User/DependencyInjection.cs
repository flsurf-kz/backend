using Flsurf.Application.User.EventHandlers;
using Flsurf.Application.User.Interfaces;

namespace Flsurf.Application.User
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<UserCreatedHandler>();

            return services;
        }
    }
}
