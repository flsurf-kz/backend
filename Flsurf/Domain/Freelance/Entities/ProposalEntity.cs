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
        public decimal Amount { get; set; }
        public CurrencyEnum Currency { get; set; }
        public string ConvertLetter { get; set; }
        public WorkTypes Type { get; set; }
        public string Status { get; set; }
        public Guid? Files { get; set; }
        public DateTime Date { get; set; }
    }

}
