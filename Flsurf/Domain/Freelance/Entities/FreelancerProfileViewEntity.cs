using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class FreelancerProfileViewEntity : BaseAuditableEntity
    {
        [ForeignKey(nameof(UserEntity))]
        public Guid FreelancerId { get; set; }
        [ForeignKey(nameof(UserEntity))]    
        public Guid ClientId { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
