namespace Flsurf.Domain.Freelance.Events
{
    public class DeadLineChanged(Guid contractId, DateTime endTime) : DomainEvent
    {
        public Guid ContractId { get; set;} = contractId;
        public DateTime DeadLine { get; set;} = endTime;
    }

    // notifies freelancer about task was added
    public class ContractTaskAdded(Guid contractId, Guid taskId) : DomainEvent
    {
        public Guid ContractId { get; set;} = contractId;
        public Guid TaskId { get; set;} = taskId;
    }

    public class ContractSignedEvent(Guid contractId, Guid freelancerId) : DomainEvent
    {
        public Guid ContractId { get; set;} = contractId;
        public Guid FreelancerId { get; set;} = freelancerId;
    }

    public class ContractSentToFreelancer : DomainEvent
    {
        public Guid ContractId { get; set;}
        public Guid FreelancerId { get; set;}

        public ContractSentToFreelancer(Guid contractId, Guid freelancerId)
        {
            ContractId = contractId;
            FreelancerId = freelancerId;
        }
    }

    public class ContractForceCancelledEvent : DomainEvent
    {
        public Guid ContractId { get; set;}
        public Guid CancelledByUserId { get; set;}
        public string Reason { get; set;}

        public ContractForceCancelledEvent(Guid contractId, Guid cancelledByUserId, string reason)
        {
            ContractId = contractId;
            CancelledByUserId = cancelledByUserId;
            Reason = reason;
        }
    }

    public class ContractCancelledByClientEvent : DomainEvent
    {
        public Guid ContractId { get; set;}
        public Guid ClientId { get; set;}
        public string Reason { get; set;}

        public ContractCancelledByClientEvent(Guid contractId, Guid clientId, string reason)
        {
            ContractId = contractId;
            ClientId = clientId;
            Reason = reason;
        }
    }

    public class FreelancerFinishedContract : DomainEvent
    {
        public Guid ContractId { get; set;}
        public Guid InitiatorId { get; set;}

        public FreelancerFinishedContract(Guid contractId, Guid initiatorId)
        {
            ContractId = contractId;
            InitiatorId = initiatorId;
        }
    }

    public class ContractFinishRejected : DomainEvent
    {
        public Guid ContractId { get; set;}
        public Guid EmployerId { get; set;}
        public string Reason { get; set;}

        public ContractFinishRejected(Guid contractId, Guid employerId, string reason)
        {
            ContractId = contractId;
            EmployerId = employerId;
            Reason = reason;
        }
    }

    public class ContractFinished(Guid contractId) : DomainEvent
    {
        public Guid ContractId { get; set; } = contractId;
    }

    public class ContractResumed(Guid contractId) : DomainEvent
    {
        public Guid ContractId { get; set;} = contractId;
    }

    public class ContractPaused(Guid contractId, string reason) : DomainEvent
    {
        public Guid ContractId { get; set;} = contractId;
        public string Reason { get; set;} = reason;
    }

    public class ContractUpdated(Guid contractId) : DomainEvent {
        public Guid ContractId { get; set;} = contractId;
    }
}
