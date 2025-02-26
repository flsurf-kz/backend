using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class FreelancerTeamEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<UserEntity> Participants { get; set; } = []; 
        public bool Closed { get; set; } = false;
        public string ClosedReason { get; set; } = null!;
        [ForeignKey("Avatar")]
        public Guid AvatarId { get; set; }
        public FileEntity Avatar { get; set; } = null!; 
    }
}
