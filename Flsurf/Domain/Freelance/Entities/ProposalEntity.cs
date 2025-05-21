using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.Payment.Enums;
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
        public decimal ProposedRate { get; set; }
        public string CoverLetter { get; set; } = null!;
        public ProposalStatus Status { get; set; }
        public ICollection<FileEntity>? Files { get; set; } = []; 
    }
}
