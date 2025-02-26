using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class JobEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; } = null!; 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<SkillEntity> RequiredSkills { get; set; } = [];  // many to many? 
        public CategoryEntity Category { get; set; } = null!;
        public decimal? Budget { get; set; } 
        public decimal? HourlyRate { get; set; }
        public DateTime? ExpirationDate { get; set; } 
        public int? Duration { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Draft;
        public ICollection<ProposalEntity> Proposals { get; set; } = []; 
        public JobLevel Level { get; set; }
        public BudgetType BudgetType { get; set; }
        public Guid? SelectedFreelancerId { get; set; }
        public DateTime? PublicationDate { get; set; } // when job status is public 
        public bool PaymentVerified { get; set; } = false; 
    }
}
