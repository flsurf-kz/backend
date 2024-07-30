using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Freelance.Entities
{
    public class FreelancerProfileEntity : BaseAuditableEntity
    {
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string Portfolio { get; set; }
        public string Resume { get; set; }
        public float HourlyRate { get; set; }
        public string Availability { get; set; }
        public float Rating { get; set; }
        public string Certificates { get; set; }
        public string Languages { get; set; }
    }
}
