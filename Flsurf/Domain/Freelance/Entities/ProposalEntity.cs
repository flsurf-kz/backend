using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ProposalEntity : BaseAuditableEntity
    {
        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public JobEntity Job { get; set; } = null!;
        [ForeignKey("User")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; } = null!;
        public Money ProposedRate { get; set; } = null!; 
        public BudgetType BudgetType { get; set; } = BudgetType.Fixed; 
        public string CoverLetter { get; set; } = null!;
        public ProposalStatus Status { get; set; }
        public ICollection<FileEntity>? Files { get; set; } = []; 
        public int EsitimatedDurationDays { get; set; }
        public string? SimilarExpriences { get; set; } = null!;
        public ICollection<PortfolioProjectEntity>? PortfolioProjects { get; set; } = []; 
    }
}
