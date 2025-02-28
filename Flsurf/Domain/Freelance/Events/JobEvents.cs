using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class JobWasBookmarked(BookmarkedJobEntity bookmark) : BaseEvent
    {
        public BookmarkedJobEntity BookmarkedJob { get; set; } = bookmark; 
    }
}
