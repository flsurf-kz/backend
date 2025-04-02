using Flsurf.Application.Files;
using Flsurf.Application.Payment;
using Flsurf.Application.Staff;
using Flsurf.Application.User;
using Flsurf.Infrastructure.EventDispatcher;
using Flsurf.Infrastructure;
using System.Reflection;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Services;

namespace Flsurf.Application.Freelance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFreelancerApplicationServices(this IServiceCollection services, ConfigurationManager config)
        {

            services.AddScoped<ITaskService, TaskService>

            return services;
        }
    }
}
