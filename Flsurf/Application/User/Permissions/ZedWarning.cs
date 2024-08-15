using SpiceDb.Models;

namespace Flsurf.Application.User.Permissions
{
    public class ZedWarning : ResourceReference
    {
        private ZedWarning(string warnId) : base($"flsurf/warning:{warnId}") { }

        public static ZedWarning WithId(Guid warnId) => new ZedWarning(warnId.ToString());

        public static ZedWarning WithWildcard() => new ZedWarning("*"); 
    }
}
