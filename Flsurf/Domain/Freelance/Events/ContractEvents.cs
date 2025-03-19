using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class DeadLineChanged(ContractEntity contract, DateTime endTime) : DomainEvent
    {
        public ContractEntity Contract { get; } = contract; 
        public DateTime DeadLine { get; } = endTime;
    }

    // notifies freelancer about task was added
    public class ContractTaskAdded(ContractEntity contract, TaskEntity task) : DomainEvent
    {  
        public ContractEntity Contrct { get; } = contract;
        public TaskEntity Task { get; } = task;
    }

    public class ContractSignedEvent : DomainEvent
    {
        public Guid ContractId { get; }
        public Guid FreelancerId { get; }

        public ContractSignedEvent(Guid contractId, Guid freelancerId)
        {
            ContractId = contractId;
            FreelancerId = freelancerId;
        }
    }

    public class ContractSentToFreelancer : DomainEvent
    {
        public Guid ContractId { get; }
        public Guid FreelancerId { get; }

        public ContractSentToFreelancer(Guid contractId, Guid freelancerId)
        {
            ContractId = contractId;
            FreelancerId = freelancerId;
        }
    }

    public class ContractForceCancelledEvent : DomainEvent
    {
        public Guid ContractId { get; }
        public Guid CancelledByUserId { get; }
        public string Reason { get; }

        public ContractForceCancelledEvent(Guid contractId, Guid cancelledByUserId, string reason)
        {
            ContractId = contractId;
            CancelledByUserId = cancelledByUserId;
            Reason = reason;
        }
    }

}
