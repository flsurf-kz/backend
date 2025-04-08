using Flsurf.Domain.Freelance.Entities;

namespace Flsurf.Domain.Freelance.Events
{
    public class JobWasBookmarked(BookmarkedJobEntity bookmark) : BaseEvent
    {
        public Guid BookmarkId { get; } = bookmark.Id;
    }

    public class ContractWasCreated(ContractEntity contract, JobEntity job) : BaseEvent
    {
        public Guid ContractId { get; } = contract.Id;
        public Guid JobId { get; } = job.Id;
    }
}
