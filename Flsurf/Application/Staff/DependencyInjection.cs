using Flsurf.Application.Staff.Interfaces;
using Flsurf.Application.Staff.UseCases;

namespace Flsurf.Application.Staff
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddStaffApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IStaffService, StaffService>();

            return services;
        }
    }
}
