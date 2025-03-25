using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedContest : ResourceReference
    {
        private ZedContest(string contestId)
            : base($"freelance/contest:{contestId}") { }

        public static ZedContest WithId(Guid contestId) => new ZedContest(contestId.ToString());

        public static ZedContest WithWildcard() => new ZedContest("*");

        public Relationship Owner(ZedFreelancerUser user)
            => new Relationship(user, "owner", this);
    }
}
