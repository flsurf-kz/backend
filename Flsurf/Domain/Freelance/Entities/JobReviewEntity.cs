using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class JobReviewEntity : BaseAuditableEntity
    {
        [ForeignKey("Reviewer")]
        public Guid ReviewerId { get; set; }
        public UserEntity Reviewer { get; set; }
        [ForeignKey("Target")]
        public Guid TargetId { get; set; }
        public UserEntity Target { get; set; }
        [ForeignKey("Job")]
        public Guid JobId { get; set; }
        public JobEntity Job { get; set; } = new JobEntity();
        public float Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
    }
}
