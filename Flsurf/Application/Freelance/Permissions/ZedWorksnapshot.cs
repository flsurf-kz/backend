using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedWorkSession : ResourceReference
    {
        private ZedWorkSession(string worksessionId) : base($"flsurf/worksession:{worksessionId}") { }

        public static ZedWorkSession WithId(Guid sessionId) => new(sessionId.ToString());

        public static ZedWorkSession WithWildcard() => new("*");

        public Relationship Owner(ZedFreelancerUser user) => new(user, "owner", this); 
    }
}
