using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedFreelancerGroup : ResourceReference
    {
        private ZedFreelancerGroup(Guid groupId) : base($"freelance/group:{groupId}") { }

        public static ZedFreelancerGroup WithId(Guid groupId) => new(groupId);

        public Relationship Member(ZedFreelanceUser user) => new(user, "member", this);
    }
}
