using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Freelance.Entities
{
    public class FreelancerTeamEntity : BaseAuditableEntity
    {
        public string Name { get; set; }
        public ICollection<UserEntity> Participants { get; set; }
        public bool Closed { get; set; }
        public string ClosedReason { get; set; }
        public Guid Avatar { get; set; }
    }
}
