using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Freelance.Queries.Responses
{
    public class SkillModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
