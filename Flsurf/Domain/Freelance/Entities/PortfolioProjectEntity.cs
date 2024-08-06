using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Freelance.Entities
{
    public class PortfolioProjectEntity : BaseAuditableEntity
    {
        public string Name { get; set; }
        public string UserRole { get; set; }
        public ICollection<SkillEntity> Skills { get; set; }
        public string Description { get; set; }
        public ICollection<Guid> Images { get; set; }
        public bool Hidden { get; set; }
    }
}
