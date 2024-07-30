using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class ContestEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid EmployerId { get; set; }
        public UserEntity Employer { get; set; }
        public string ContestTitle { get; set; }
        public string ContestDescription { get; set; }
        public float PrizePool { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContestStatus Status { get; set; }
    }
}
