using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Freelance.Events
{
    public class DeadLineChanged(ContractEntity contract, DateTime endTime) : BaseEvent
    {
        public ContractEntity Contract { get; } = contract; 
        public DateTime DeadLine { get; } = endTime;
    }

    // notifies freelancer about task was added
    public class ContractTaskAdded(ContractEntity contract, TaskEntity task) : BaseEvent {  
        public ContractEntity Contrct { get; } = contract;
        public TaskEntity Task { get; } = task;
    }
}
