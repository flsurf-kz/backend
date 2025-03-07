using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class JobWasBookmarked(BookmarkedJobEntity bookmark) : BaseEvent
    {
        public BookmarkedJobEntity BookmarkedJob { get; set; } = bookmark; 
    }

    public class ContractWasCreated(ContractEntity contract, JobEntity job) : BaseEvent
    {
        public ContractEntity Contract { get; set; } = contract;
        public JobEntity Job { get; set; } = job;
    }
}
