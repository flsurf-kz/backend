using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedContract : ResourceReference
    {
        private ZedContract(string contractId) : base($"flsurf/contract:{contractId}") { }

        public static ZedContract WithId(Guid contractId) => new(contractId.ToString());

        public static ZedContract WithWildCard() => new("*"); 

        public Relationship Client(ZedFreelancerUser user) => new(user, "client", this);

        public Relationship Freelancer(ZedFreelancerUser user) => new(user, "freelancer", this);

        public Relationship FreelancerGroup(ZedFreelancerTeam group) => new(group, "freelancer", this);

        public Relationship Overwatcher(ZedFreelancerUser user) => new(user, "overwatcher", this);
    }
}
