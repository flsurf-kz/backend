using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class JobQueries
    {
        public static IQueryable<JobEntity> IncludeStandard(this IQueryable<JobEntity> query)
        {
            return query
                .Include(x => x.Proposals)
                .Include(x => x.RequiredSkills)
                .Include(x => x.Category)
                .Include(x => x.Employer);
        }
    }
}
