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
        public PortfolioProjectEntity[] PortfolioProjects { get; set; } = []; 
        public string Resume { get; set; }
        public float HourlyRate { get; set; } 
        public string Availability { get; set; } // TODO 
        public float Rating { get; set; }
        //public string Certificates { get; set; }  // TODO
        public string[] Languages { get; set; }
    }
}
