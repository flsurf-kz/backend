using Flsurf.Domain.Freelance.Entities;

namespace Flsurf.Domain.Freelance.Events
{
    public class JobWasBookmarked(Guid bookmarkId) : BaseEvent
    {
        public Guid BookmarkId { get; } = bookmarkId;
    }

    public class ContractWasCreated(Guid contractId, Guid jobId) : BaseEvent
    {
        public Guid ContractId { get; } = contractId;
        public Guid JobId { get; } = jobId;
    }
}
