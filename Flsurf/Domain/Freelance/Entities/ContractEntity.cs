using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Diagnostics.Contracts;
using Flsurf.Application.Common.Exceptions;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ContractEntity : BaseAuditableEntity
    {
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; } = null!;

        [ForeignKey("Employer")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public decimal? CostPerHour { get; set; }
        public BudgetType BudgetType { get; set; }

        public ICollection<TaskEntity> Tasks { get; set; } = [];
        public ICollection<WorkSessionEntity> WorkSessions { get; set; } = [];

        // Динамически вычисляемые свойства
        public int TotalWorkSessions => WorkSessions?.Count ?? 0;

        public decimal TotalHoursWorked => WorkSessions?
            .Where(ws => ws.EndDate.HasValue)
            .Sum(ws => (decimal)(ws.EndDate.Value - ws.StartDate).TotalHours) ?? 0;

        public decimal? RemainingBudget => Budget.HasValue && CostPerHour.HasValue
            ? Budget - (TotalHoursWorked * CostPerHour)
            : null;

        public PaymentScheduleType PaymentSchedule { get; set; }
        public DisputeStatus? DisputeStatus { get; set; }
        public bool IsPaused { get; set; } = false;
        public string? PauseReason { get; set; }
        public string ContractTerms { get; set; } = string.Empty;
        public decimal? Bonus { get; set; }

        public void ChangeDeadline(DateTime endTime)
        {
            EndDate = endTime;
            AddDomainEvent(new DeadLineChanged(this, endTime));
        }

        public void AddTask(TaskEntity task)
        {
            Tasks.Add(task);
            AddDomainEvent(new ContractTaskAdded(this, task));
        }

        public void PauseContract(string reason)
        {
            IsPaused = true;
            PauseReason = reason;
            AddDomainEvent(new ContractPaused(this, reason));
        }

        public void ResumeContract()
        {
            IsPaused = false;
            PauseReason = null;
            AddDomainEvent(new ContractResumed(this));
        }

        public void CancelContract()
        {
            if (Status == ContractStatus.Cancelled)
                throw new DomainException("Контракт уже отменен");
            Status = ContractStatus.Cancelled;
            EndDate = DateTime.UtcNow;
            PauseReason = command.Reason;
            IsPaused = false;
        }
    }

}
