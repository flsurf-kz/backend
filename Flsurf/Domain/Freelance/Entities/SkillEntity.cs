using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Entities
{
    public class SkillEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        //[ForeignKey("Category")]
        //public Guid CategoryId { get; set; }
        //public CategoryEntity Category { get; set; } = null!;
        [JsonIgnore]
        // DONT EAGERLY LOAD AT ANY circumstances
        public ICollection<JobEntity> Jobs { get; set; } = [];
        
        public static SkillEntity Create(string name)
        {
            return new SkillEntity
            {
                Name = name,
            }
        }
    }
}
