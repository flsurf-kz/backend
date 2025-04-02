using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Freelance.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class TaskEntity : BaseAuditableEntity
    {
        [ForeignKey("Contract")]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; } = null!;

        public string TaskTitle { get; private set; } = null!;
        public string TaskDescription { get; private set; } = null!;
        public string Status { get; private set; } = "Pending";
        public int Priority { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? CompletionDate { get; private set; }

        private TaskEntity() { }

        public static TaskEntity Create(Guid contractId, string title, string description, int priority)
        {
            return new TaskEntity
            {
                ContractId = contractId,
                TaskTitle = title,
                TaskDescription = description,
                Priority = priority,
                Status = "Pending",
                CreationDate = DateTime.UtcNow
            };
        }

        public void Complete()
        {
            if (Status == "Completed")
                throw new DomainException("Задача уже завершена");

            if (Status == "Approved")
                throw new DomainException("Задача уже одобрена");

            Status = "Completed";
            CompletionDate = DateTime.UtcNow;
            AddDomainEvent(new TaskCompletedEvent(this));
        }

        public void Approve()
        {
            if (Status != "Completed")
                throw new DomainException("Нельзя одобрить задачу, которая ещё не завершена");

            Status = "Approved";
            AddDomainEvent(new TaskApprovedEvent(this));
        }

        public void RequestChanges(string newTitle, string newDescription)
        {
            if (Status != "Completed")
                throw new DomainException("На доработку можно отправить только завершённую задачу");

            TaskTitle = newTitle;
            TaskDescription = newDescription;
            Status = "Revision";
            CompletionDate = null;

            AddDomainEvent(new TaskSentForRevisionEvent(this));
        }

        public void Update(string newTitle, string newDescription, int newPriority)
        {
            if (Status == "Approved")
                throw new DomainException("Нельзя редактировать одобренную задачу");

            TaskTitle = newTitle;
            TaskDescription = newDescription;
            Priority = newPriority;

            AddDomainEvent(new TaskUpdatedEvent(this));
        }

        public bool IsCompleted => Status == "Completed";
        public bool IsApproved => Status == "Approved";
        public bool IsInRevision => Status == "Revision";
    }
}
