using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Freelance.Events;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Entities
{
    // Job is resonsible for public view, and everything before contract sign. 
    // then all tasks, resign, rejects and etc are COntract resposnibilities 
    public class JobEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; } = null!; 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<SkillEntity> RequiredSkills { get; set; } = [];  // many to many? 
        public CategoryEntity Category { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Money Payout { get; set; } = Money.Null();  
        public DateTime? ExpirationDate { get; set; } 
        public int? Duration { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Draft;
        public ICollection<ProposalEntity> Proposals { get; set; } = []; 
        public JobLevel Level { get; set; }
        public BudgetType BudgetType { get; set; }
        public DateTime? PublicationDate { get; set; } // when job status is public 
        public bool PaymentVerified { get; set; } = false;
        [JsonIgnore]
        public ContractEntity? Contract { get; set; } 
        [JsonIgnore]
        [ForeignKey("Contract")]
        public Guid? ContractId { get; private set; }
        public ICollection<FileEntity> Files { get; private set; } = [];
        public bool IsHidden { get; set; } = false; 

        public static JobEntity CreateFixed(
            UserEntity employer,
            string title,
            string description,
            Money budget,
            bool paymentVerified, 
            JobLevel level,
            ICollection<SkillEntity> skills, 
            ICollection<FileEntity> files, 
            CategoryEntity category)
        {
            return new JobEntity()
            {
                Payout = budget,
                Title = title,
                Description = description,
                PaymentVerified = paymentVerified,
                Level = level,
                RequiredSkills = skills,
                Employer = employer,
                EmployerId = employer.Id,
                BudgetType = BudgetType.Fixed,
                Category = category,
                Files = files, 
            }; 
        }

        public static JobEntity CreateHourly(
            UserEntity employer,
            string title,
            string description,
            Money horlyRate,
            bool paymentVerified,
            JobLevel level,
            ICollection<SkillEntity> skills,
            ICollection<FileEntity> files,
            CategoryEntity category)
        {
            return new JobEntity()
            {
                Payout = horlyRate,
                Title = title,
                Description = description,
                PaymentVerified = paymentVerified,
                Level = level,
                Files = files, 
                RequiredSkills = skills,
                Employer = employer,
                EmployerId = employer.Id,
                BudgetType = BudgetType.Hourly,
                Category = category,
            };
        }

        public void SetFiles(ICollection<FileEntity> files)
        {
            Files = files; 
        }

        // decimal bc you cant change currency
        public void UpdateBudget(decimal amount)
        {
            if (amount == 0)
                return;  
            if (BudgetType != BudgetType.Fixed)
                throw new DomainException("не правильный тип бюджета");
            Payout = new Money(amount);
        }

        public void UpdateHourlyRate(decimal amount)  
        {
            if (amount == 0)
                return;
            if (BudgetType != BudgetType.Hourly)
                throw new DomainException("не правильный тип бюджета");
            Payout = new Money(amount);
        }
    }
}
