namespace Flsurf.Domain.Freelance.Events
{
    public class ReactedToProposal(Guid proposalId) : BaseEvent
    {
        public Guid ProposalId { get; set; } = proposalId; 
    }
}
