using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedSkill : ResourceReference
    {
        private ZedSkill(string skillId) : base($"freelance/user:{skillId}") { }

        public static ZedSkill WithId(Guid id) => new(id.ToString());

        public static ZedSkill WithWildcard() => new("*");
    }

}
