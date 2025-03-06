using Flsurf.Domain.Common;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class WorkSessionStarted(WorkSessionEntity ent, DateTime startedAt) : BaseEvent
    {
        public DateTime StartedAt { get; } = startedAt;
        public WorkSessionEntity WorkSessionEntity { get; } = ent; 
    }

    public class WorkSessionEnded(WorkSessionEntity ent, DateTime endedAt) : BaseEvent
    {
        public DateTime EndedAt { get; } = endedAt;
        public WorkSessionEntity WorkSessionEntity { get; } = ent;
    }

    public class WorkSessionSubmitted(WorkSessionEntity session, DateTime submittedAt) : BaseEvent
    {
        public DateTime SubmittedAt { get; } = submittedAt;
        public WorkSessionEntity WorkSession { get; } = session;
    }

    public class WorkSessionApproved(WorkSessionEntity session, DateTime approvedAt) : BaseEvent
    {
        public DateTime ApprovedAt { get; } = approvedAt;
        public WorkSessionEntity WorkSession { get; } = session;
    }

    public class WorkSessionRejected(WorkSessionEntity session, DateTime rejectedAt, string reason) : BaseEvent
    {
        public DateTime RejectedAt { get; } = rejectedAt;
        public WorkSessionEntity WorkSession { get; } = session;
        public string Reason { get; } = reason;
    }
}
