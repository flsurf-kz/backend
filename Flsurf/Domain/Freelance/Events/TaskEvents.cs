using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class TaskCreatedEvent(TaskEntity task) : BaseEvent
    {
        public TaskEntity Task { get; } = task; 
    }

    public class TaskCompletedEvent(TaskEntity task) : BaseEvent
    {
        public TaskEntity Task { get; } = task; 
    }

    public class TaskApprovedEvent(TaskEntity task) : BaseEvent
    {
        public TaskEntity Task { get; } = task; 
    }

    public class TaskSentForRevisionEvent(TaskEntity task) : BaseEvent
    {
        public TaskEntity Task { get; } = task; 
    }

    public class TaskUpdatedEvent(TaskEntity task) : BaseEvent
    {
        public TaskEntity Task { get; } = task; 
    }
}
