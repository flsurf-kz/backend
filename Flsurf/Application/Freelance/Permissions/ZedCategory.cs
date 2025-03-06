using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Permissions
{
    public class ZedCategory : ResourceReference
    {
        private ZedCategory(string categoryId) : base($"freelance/freelance_category:{categoryId}") { }

        public static ZedCategory WithWildcard() => new("*"); 

        public static ZedCategory WithId(Guid categoryId) => new(categoryId.ToString());
    }
}
