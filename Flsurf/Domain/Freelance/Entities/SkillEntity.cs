using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class SkillEntity : BaseAuditableEntity
    {
        public string Name { get; set; }
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public CategoryEntity Category { get; set; }
    }
}
