using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedClientProfile : ResourceReference
    {
        private ZedClientProfile(Guid profileId) : base($"freelance/profile:{profileId}") { }

        public static ZedClientProfile WithId(Guid profileId) => new(profileId);
    }
}
