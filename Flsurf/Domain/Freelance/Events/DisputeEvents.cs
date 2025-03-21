using Flsurf.Application.Freelance.Commands.Contract;
using Flsurf.Domain.Freelance.Enums;

namespace Flsurf.Domain.Freelance.Events
{
    public class DisputeStatusChangedEvent : DomainEvent
    {
        public Guid DisputeId { get; }
        public DisputeStatus NewStatus { get; }

        public DisputeStatusChangedEvent(Guid disputeId, DisputeStatus newStatus)
        {
            DisputeId = disputeId;
            NewStatus = newStatus;
        }
    }

    public class DisputeInitiatedEvent : DomainEvent
    {
        public Guid DisputeId { get; }
        public Guid TicketId { get; }
        public Guid ContractId { get; }

        public DisputeInitiatedEvent(Guid disputeId, Guid ticketId, Guid contractId)
        {
            DisputeId = disputeId;
            TicketId = ticketId;
            ContractId = contractId;
        }
    }

    public class DisputeAcceptedEvent : DomainEvent
    {
        public Guid DisputeId { get; }
        public Guid ModeratorId { get; }

        public DisputeAcceptedEvent(Guid disputeId, Guid moderatorId)
        {
            DisputeId = disputeId;
            ModeratorId = moderatorId;
        }
    }

    public class DisputeResolvedEvent : DomainEvent
    {
        public Guid DisputeId { get; }
        public Guid ModeratorId { get; }
        public DisputeResolutionStrategy ResolutionStrategy { get; }

        public DisputeResolvedEvent(Guid disputeId, Guid moderatorId, DisputeResolutionStrategy resolutionStrategy)
        {
            DisputeId = disputeId;
            ModeratorId = moderatorId;
            ResolutionStrategy = resolutionStrategy;
        }
    }
}
