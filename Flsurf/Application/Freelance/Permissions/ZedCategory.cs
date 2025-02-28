using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedCategory : ResourceReference
    {
        private ZedCategory(Guid categoryId) : base($"freelance/freelance_category:{categoryId}") { }

        public static ZedCategory WithId(Guid categoryId) => new(categoryId);
    }
}
