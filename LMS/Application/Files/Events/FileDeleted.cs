using Flsurf.Domain.Files.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Application.Files.Events
{
    public class FileDeleted(FileEntity file) : BaseEvent
    {
        public FileEntity File { get; set; } = file;
    }
}
