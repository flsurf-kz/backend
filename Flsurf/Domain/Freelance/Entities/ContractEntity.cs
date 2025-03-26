﻿using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Diagnostics.Contracts;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Payment.ValueObjects;

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
        public Money? Budget { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public Money? CostPerHour { get; set; }
        public BudgetType BudgetType { get; set; }

        public ICollection<TaskEntity> Tasks { get; set; } = [];
        public ICollection<WorkSessionEntity> WorkSessions { get; set; } = [];

        // Динамически вычисляемые свойства
        public int TotalWorkSessions => WorkSessions?.Count ?? 0;

        public decimal TotalHoursWorked => WorkSessions?
            .Where(ws => ws.EndDate.HasValue)
            .Sum(ws => (decimal)(ws.EndDate.Value - ws.StartDate).TotalHours) ?? 0;

        public Money? RemainingBudget => Budget != null && CostPerHour != null
            ? Budget - (CostPerHour * TotalHoursWorked)
            : null;

        public PaymentScheduleType PaymentSchedule { get; set; }
        public bool IsPaused { get; set; } = false;
        public string? PauseReason { get; set; }
        public string ContractTerms { get; set; } = string.Empty;
        public decimal? Bonus { get; set; }
        [ForeignKey(nameof(DisputeEntity))]
        public Guid? DisputeId { get; set; }

        public static ContractEntity CreateFixed(
            Guid employerId,
            Guid freelancerId,
            decimal? budget,
            PaymentScheduleType paymentSchedule,
            string contractTerms,
            DateTime? endDate)
        {
            if (budget == null)
                throw new ArgumentException("Для фиксированного контракта необходимо указать бюджет.");

            return new ContractEntity
            {
                EmployerId = employerId,
                FreelancerId = freelancerId,
                Budget = new Money(budget ?? 0),
                CostPerHour = null,
                BudgetType = BudgetType.Fixed,
                PaymentSchedule = paymentSchedule,
                ContractTerms = contractTerms,
                StartDate = DateTime.UtcNow,
                EndDate = endDate,
                Status = ContractStatus.PendingApproval
            };
        }

        public static ContractEntity CreateHourly(
            Guid employerId,
            Guid freelancerId,
            decimal? costPerHour,
            PaymentScheduleType paymentSchedule,
            string contractTerms,
            DateTime? endDate)
        {
            if (costPerHour == null)
                throw new ArgumentException("Для почасового контракта необходимо указать ставку.");

            return new ContractEntity
            {
                EmployerId = employerId,
                FreelancerId = freelancerId,
                Budget = null,
                CostPerHour = new Money(costPerHour ?? 0),
                BudgetType = BudgetType.Hourly,
                PaymentSchedule = paymentSchedule,
                ContractTerms = contractTerms,
                StartDate = DateTime.UtcNow,
                EndDate = endDate,
                Status = ContractStatus.PendingApproval
            };
        }

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

            AddDomainEvent(new ContractFinished(this)); 
        }
    }

}
