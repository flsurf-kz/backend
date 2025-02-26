using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class PortfolioProjectEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public ICollection<SkillEntity> Skills { get; set; } = []; 
        public string Description { get; set; } = string.Empty;
        public ICollection<FileEntity> Images { get; set; } = [];
        public bool Hidden { get; set; } = true; 
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; } = null!;  
    }
}
