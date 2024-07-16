using Flsurf.Domain.Study.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class InstitutionQueries 
    {
        public static IQueryable<InstitutionEntity> IncludeStandard(this IQueryable<InstitutionEntity> query)
        {
            return query
                .Include(x => x.Images); 
        }
    }
}
