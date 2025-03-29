using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Freelance.Enums;

namespace Flsurf.Domain.Freelance.Entities
{
    public class FreelancerProfileEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public ICollection<SkillEntity> Skills { get; set; } = new List<SkillEntity>();
        public string Experience { get; set; } = string.Empty;
        public ICollection<PortfolioProjectEntity> PortfolioProjects { get; set; } = new List<PortfolioProjectEntity>();

        public string? Resume { get; set; }
        public decimal CostPerHour { get; set; }

        public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Open;
        public float Rating { get; set; } = 0;
        public bool IsHidden { get; set; } = false;
        public ICollection<JobReviewEntity> Reviews { get; set; } = []; 


        // ✅ Статический метод `.Create()`
        public static FreelancerProfileEntity Create(Guid userId, string experience, decimal hourlyRate, string? resume)
        {
            return new FreelancerProfileEntity
            {
                UserId = userId,
                Experience = experience,
                CostPerHour = hourlyRate,
                Resume = resume, 
            };
        }

        // 🏷 Добавление навыков
        public void AddSkills(IEnumerable<SkillEntity> skills)
        {
            foreach (var skill in skills)
            {
                if (!Skills.Contains(skill))
                    Skills.Add(skill);
            }
        }
    }

}
