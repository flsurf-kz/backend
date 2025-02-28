using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Commands.Job.BookmarkJob
{
    public class BookmarkJobCommand : BaseCommand
    {
        public Guid JobId { get; set; }
    }
}
