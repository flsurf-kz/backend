using Flsurf.Domain.Files.Entities;

namespace Flsurf.Application.Files.Events
{
    public class FileDeleted(FileEntity file) : BaseEvent
    {
        public FileEntity File { get; set; } = file;
    }
}
