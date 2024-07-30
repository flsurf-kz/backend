using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class WorkSnapshotEntity : BaseAuditableEntity
    {
        public DateTime Date { get; set; }
        public ICollection<Guid> Files { get; set; }
        [ForeignKey("Contract")]
        public Guid ContractId { get; set; }
        public ContractEntity Contract { get; set; }
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public UserEntity Freelancer { get; set; }
    }
}
