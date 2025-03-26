using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedFreelancerTeam : ResourceReference
    {
        private ZedFreelancerTeam(Guid teamId) : base($"flsurf/team:{teamId}") { }

        public static ZedFreelancerTeam WithId(Guid teamId) => new(teamId);

        public Relationship Member(ZedFreelancerUser user) => new(user, "member", this);

        public Relationship Owner(ZedFreelancerUser user) => new(user, "owner", this);
    }
}
