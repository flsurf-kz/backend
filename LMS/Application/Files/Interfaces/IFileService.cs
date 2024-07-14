using Flsurf.Application.Files.UseCases;

namespace Flsurf.Application.Files.Interfaces
{
    public interface IFileService
    {
        UploadFile UploadFile();
        UploadFiles UploadFiles();
        DeleteFile DeleteFile();
    }
}
