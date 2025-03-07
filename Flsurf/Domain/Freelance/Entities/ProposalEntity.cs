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
        public JobEntity Job { get; set; }
        [ForeignKey("User")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; }
        public decimal ProposedRate { get; set; }
        public string CoverLetter { get; set; }
        public ProposalStatus Status { get; set; }
        public Guid? Files { get; set; }
    }
}
