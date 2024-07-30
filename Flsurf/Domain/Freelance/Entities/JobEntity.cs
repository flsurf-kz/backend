using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class JobEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequiredSkills { get; set; }
        public float Budget { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public int NumberOfProposals { get; set; }
        public JobLevel Level { get; set; }
        public Guid? SelectedFreelancerId { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool PaymentVerified { get; set; }
    }
}
