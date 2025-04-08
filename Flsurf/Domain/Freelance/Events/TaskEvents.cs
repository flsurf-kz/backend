using Flsurf.Domain.Freelance.Entities;

namespace Flsurf.Domain.Freelance.Events
{

    public class TaskCreatedEvent(TaskEntity task) : BaseEvent
    {
        public Guid TaskId { get; } = task.Id;
    }

    public class TaskCompletedEvent(TaskEntity task) : BaseEvent
    {
        public Guid TaskId { get; } = task.Id;
    }

    public class TaskApprovedEvent(TaskEntity task) : BaseEvent
    {
        public Guid TaskId { get; } = task.Id;
    }

    public class TaskSentForRevisionEvent(TaskEntity task) : BaseEvent
    {
        public Guid TaskId { get; } = task.Id;
    }

    public class TaskUpdatedEvent(TaskEntity task) : BaseEvent
    {
        public Guid TaskId { get; } = task.Id;
    }
}
