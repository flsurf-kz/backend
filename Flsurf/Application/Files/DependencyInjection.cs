using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Files.UseCases;

namespace Flsurf.Application.Files
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFilesApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IFileService, FileService>();

            return services;
        }
    }
}
