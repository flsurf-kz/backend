using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedContract : ResourceReference
    {
        private ZedContract(Guid contractId) : base($"freelance/contract:{contractId}") { }

        public static ZedContract WithId(Guid contractId) => new(contractId);

        public Relationship Client(ZedFreelanceUser user) => new(user, "client", this);

        public Relationship Freelancer(ZedFreelanceUser user) => new(user, "freelancer", this);

        public Relationship FreelancerGroup(ZedFreelancerGroup group) => new(group, "freelancer", this);

        public Relationship Overwatcher(ZedFreelanceUser user) => new(user, "overwatcher", this);
    }
}
