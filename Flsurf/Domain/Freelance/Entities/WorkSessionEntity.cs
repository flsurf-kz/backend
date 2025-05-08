using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Entities
{

    public class WorkSessionEntity : BaseAuditableEntity
    {
        public ICollection<FileEntity>? Files { get; set; } = [];  // 
        [ForeignKey("Contract")]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; } = null!;
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // тут не будет трекинга актива, это просто флаг внутри тех стороны а не для бизнес логики
        [Newtonsoft.Json.JsonIgnore]
        public bool IsActive { get; set; } = false; 

        public string? Comment { get; set; } = string.Empty;

        public string? ClientComment { get; set; }

        public DateTime? SubmittedAt { get; set; } // Когда сессия отправлена клиенту
        public DateTime? ApprovedAt { get; set; } // Когда клиент одобрил
        public DateTime? RejectedAt { get; set; } // Когда клиент отказал
        public WorkSessionStatus Status { get; set; } = WorkSessionStatus.Pending; // Статус сессии

        // ⏳ Если клиент не ответил за 2 дня, автопринятие
        public bool AutoApproved => SubmittedAt.HasValue && (DateTime.UtcNow - SubmittedAt.Value).TotalHours >= 48;

        public static WorkSessionEntity Create(UserEntity freelancer, ContractEntity contract)
        {
            var session = new WorkSessionEntity
            {
                ClientComment = string.Empty,
                StartDate = DateTime.Now,
                Freelancer = freelancer,
                Contract = contract,
                ContractId = contract.Id,
                FreelancerId = freelancer.Id,
            };

            return session; 
        }

        public int WorkedHours()
        {
            if (EndDate == null)
                return 0;

            return (int)(EndDate.Value - StartDate).TotalHours; // Фикс
        }


        public Money TotalEarned()
        {
            if (Contract == null)
                throw new NullReferenceException("Contract is not loaded dumbass");

            if (Contract.BudgetType != BudgetType.Hourly || Contract.CostPerHour == Money.Null())
                return Money.Null();

            if (EndDate == null)
                return new Money(0); // Фикс

            return Contract.CostPerHour * WorkedHours();
        }

        public IEnumerable<DateOnly> GetWorkDates()
        {
            if (EndDate is null)
                yield break;

            // нормализуем к полуночи UTC
            var current = StartDate.Date;
            var last = EndDate.Value.Date;

            while (current <= last)
            {
                yield return DateOnly.FromDateTime(current);
                current = current.AddDays(1);
            }
        }

        public void StartSession()
        {
            if (IsActive) return;
            IsActive = true;
            StartDate = DateTime.UtcNow; // Устанавливаем время старта
            AddDomainEvent(new WorkSessionStarted(this, StartDate));
        }

        public void EndSession(List<FileEntity> files)
        {
            if (!IsActive) return;

            if (files.Count < WorkedHours())
                throw new DomainException("Должно быть минимум 1 фото за час работы");

            IsActive = false;
            EndDate = DateTime.UtcNow;
            Files = files;
            Status = WorkSessionStatus.Pending;
            SubmittedAt = DateTime.UtcNow;

            AddDomainEvent(new WorkSessionSubmitted(this, DateTime.Now));
        }

        public void ApproveSession()
        {
            if (Status != WorkSessionStatus.Pending)
                throw new DomainException("Сессия уже обработана");

            Status = WorkSessionStatus.Approved;
            ApprovedAt = DateTime.UtcNow;
            AddDomainEvent(new WorkSessionApproved(this, DateTime.Now));
        }

        public void RejectSession(string clientComment)
        {
            if (Status != WorkSessionStatus.Pending)
                throw new DomainException("Сессия уже обработана");

            Status = WorkSessionStatus.Rejected;
            RejectedAt = DateTime.UtcNow;
            ClientComment = clientComment;

            AddDomainEvent(new WorkSessionRejected(this, DateTime.Now, clientComment));
        }
    }
}
