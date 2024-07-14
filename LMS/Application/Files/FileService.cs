using Flsurf.Application.Files.Interfaces;
using Flsurf.Application.Files.UseCases;

namespace Flsurf.Application.Files
{
    public class FileService : IFileService
    {
        private readonly IServiceProvider ServiceProvider;

        public FileService(
            IServiceProvider serviceProvider
        )
        {
            ServiceProvider = serviceProvider;
        }

        public UploadFile UploadFile()
        {
            return ServiceProvider.GetRequiredService<UploadFile>();
        }

        public UploadFiles UploadFiles()
        {
            return ServiceProvider.GetRequiredService<UploadFiles>();
        }

        public DeleteFile DeleteFile()
        {
            return ServiceProvider.GetRequiredService<DeleteFile>();
        }
    }
}
