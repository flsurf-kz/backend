using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ContractEntity : BaseAuditableEntity
    {
        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        public JobEntity Job { get; set; } = null!;
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; } = null!;
        [ForeignKey("Employer")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; } = null!; 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? ContractMoney { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Active;
        public decimal? CostPerHour { get; set; }
        public BudgetType BudgetType { get; set; }
    }
}
