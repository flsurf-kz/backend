using Flsurf.Domain.Freelance.Enums;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{   
    // anemic entity, FreelancerTeam has ownership over this entity! 
    public class FreelancerTeamInvitation : BaseAuditableEntity
    {
        [ForeignKey("FreelancerTeam")]
        public Guid FreelancerTeamId { get; set; }
        public FreelancerTeamEntity FreelancerTeam { get; set; } = null!;  
        [ForeignKey("User")] 
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public TeamInvitationStatus Status { get; set; } = TeamInvitationStatus.Waiting; 

        public static FreelancerTeamInvitation Create(UserEntity user, FreelancerTeamEntity team)
        {
            return new FreelancerTeamInvitation() { FreelancerTeam =  team, User = user };
        }
    }
}
