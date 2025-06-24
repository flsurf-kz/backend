using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Payment.ValueObjects;
using System.Text.Json.Serialization;
using Flsurf.Domain.Files.Entities;

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
        [ForeignKey(nameof(ProposalEntity))]
        public Guid ProposalId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // Use Money.Null for comparisions against Null 
        public Money Budget { get; set; } = Money.Null(); // костыль 
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        // Use Money.Null for comparisions against Null 
        public Money CostPerHour { get; set; } = Money.Null(); // костыль  
        public BudgetType BudgetType { get; set; }

        public ICollection<TaskEntity> Tasks { get; set; } = [];
        public ICollection<WorkSessionEntity> WorkSessions { get; set; } = [];

        // Динамически вычисляемые свойства
        public int TotalWorkSessions => WorkSessions?.Count ?? 0;
        [JsonIgnore]
        public ICollection<BonusEntity> Bonuses { get; set; } = []; 


        // NEED FIX this edgecase of nullable!! 
        public decimal TotalHoursWorked => WorkSessions?
            .Where(ws => ws.EndDate.HasValue)
            .Sum(ws => (decimal)(ws.EndDate!.Value - ws.StartDate).TotalHours) ?? 0;  

        public Money? RemainingBudget => Budget != Money.Null() && CostPerHour != Money.Null()
            ? Budget - (CostPerHour * TotalHoursWorked)
            : null;

        public PaymentScheduleType PaymentSchedule { get; set; }
        public bool IsPaused { get; set; } = false;
        public string? PauseReason { get; set; }
        public string ContractTerms { get; set; } = string.Empty;
        public decimal? Bonus { get; set; }
        [ForeignKey(nameof(DisputeEntity))]
        public Guid? DisputeId { get; set; }

        public JobEntity Job { get; set; } = null!; 
        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        public List<FileEntity> Files { get; set; } = []; 

        public static ContractEntity CreateFixed(
            Guid employerId,
            Guid freelancerId,
            Money budget,
            PaymentScheduleType paymentSchedule,
            string contractTerms,
            Guid jobId,
            Guid proposalId,
            DateTime? endDate)
        {
            return new ContractEntity
            {
                EmployerId = employerId,
                FreelancerId = freelancerId,
                Budget = budget,
                CostPerHour = Money.Null(),
                BudgetType = BudgetType.Fixed,
                PaymentSchedule = paymentSchedule,
                ContractTerms = contractTerms,
                StartDate = DateTime.UtcNow,
                ProposalId = proposalId,
                EndDate = endDate,
                Status = ContractStatus.PendingApproval, 
                JobId = jobId, 
            };
        }

        public static ContractEntity CreateHourly(
            Guid employerId,
            Guid freelancerId,
            Money costPerHour,
            PaymentScheduleType paymentSchedule,
            string contractTerms,
            Guid jobId, 
            Guid proposalId, 
            DateTime? endDate)
        {
            return new ContractEntity
            {
                EmployerId = employerId,
                FreelancerId = freelancerId,
                Budget = Money.Null(),
                CostPerHour = costPerHour,
                BudgetType = BudgetType.Hourly,
                PaymentSchedule = paymentSchedule,
                ContractTerms = contractTerms,
                StartDate = DateTime.UtcNow,
                ProposalId = proposalId,
                EndDate = endDate,
                Status = ContractStatus.PendingApproval, 
                JobId = jobId, 
            };
        }

        public void ChangeDeadline(DateTime endTime)
        {
            EndDate = endTime;
            AddDomainEvent(new DeadLineChanged(Id, endTime));
        }

        public void AddTask(TaskEntity task)
        {
            Tasks.Add(task);
            AddDomainEvent(new ContractTaskAdded(Id, task.Id));
        }

        public void PauseContract(string reason)
        {
            IsPaused = true;
            PauseReason = reason;
            AddDomainEvent(new ContractPaused(this.Id, reason));
        }

        public void ResumeContract()
        {
            IsPaused = false;
            PauseReason = null;
            AddDomainEvent(new ContractResumed(this.Id));
        }

        public void StartDispute()
        {
            IsPaused = true;
            PauseReason = "Конфликт интересов";
            Status = ContractStatus.Paused;

        }

        public void CancelContract()
        {
            if (Status == ContractStatus.Cancelled)
                throw new DomainException("Контракт уже отменен");
            Status = ContractStatus.Cancelled;
            EndDate = DateTime.UtcNow;
            PauseReason = "Контракт отменен";
            IsPaused = false;
        }

        public void Finish()
        {
            if (Status != ContractStatus.PendingFinishApproval)
                throw new DomainException("Контракт не в активном состоянии");
            Status = ContractStatus.Completed; 
            EndDate = DateTime.UtcNow;
            IsPaused = true;
            PauseReason = "Контракт завершен";

            AddDomainEvent(new ContractFinished(this.Id)); 
        }
    }

}
