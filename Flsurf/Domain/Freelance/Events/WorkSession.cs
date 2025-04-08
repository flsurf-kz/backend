using Flsurf.Domain.Freelance.Entities;

namespace Flsurf.Domain.Freelance.Events
{

    public class WorkSessionStarted(WorkSessionEntity session, DateTime startedAt) : BaseEvent
    {
        public Guid WorkSessionId { get; } = session.Id;
        public DateTime StartedAt { get; } = startedAt;
    }


    public class WorkSessionEnded(WorkSessionEntity session, DateTime endedAt) : BaseEvent
    {
        public Guid WorkSessionId { get; } = session.Id;
        public DateTime EndedAt { get; } = endedAt;
    }

    public class WorkSessionSubmitted(WorkSessionEntity session, DateTime submittedAt) : BaseEvent
    {
        public Guid WorkSessionId { get; } = session.Id;
        public DateTime SubmittedAt { get; } = submittedAt;
    }

    public class WorkSessionApproved(WorkSessionEntity session, DateTime approvedAt) : BaseEvent
    {
        public Guid WorkSessionId { get; } = session.Id;
        public DateTime ApprovedAt { get; } = approvedAt;
    }

    public class WorkSessionRejected(WorkSessionEntity session, DateTime rejectedAt, string reason) : BaseEvent
    {
        public Guid WorkSessionId { get; } = session.Id;
        public DateTime RejectedAt { get; } = rejectedAt;
        public string Reason { get; } = reason;
    }
}
