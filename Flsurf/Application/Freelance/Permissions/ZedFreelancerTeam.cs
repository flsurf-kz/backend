using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedFreelancerTeam : ResourceReference
    {
        private ZedFreelancerTeam(Guid groupId) : base($"freelance/group:{groupId}") { }

        public static ZedFreelancerTeam WithId(Guid groupId) => new(groupId);

        public Relationship Member(ZedFreelanceUser user) => new(user, "member", this);

        public Relationship Owner(ZedFreelanceUser user) => new(user, "owner", this);
    }
}
