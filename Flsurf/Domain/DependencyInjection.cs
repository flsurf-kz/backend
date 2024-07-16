namespace Flsurf.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            //services.AddScoped<AssigmentService>(); 
            //services.AddScoped<PermissionService>(); 

            return services;
        }
    }
}
