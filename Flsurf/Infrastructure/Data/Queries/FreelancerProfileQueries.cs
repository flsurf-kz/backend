using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class FreelancerProfileQueries
    {
        public static IQueryable<FreelancerProfileEntity> IncludeStandard(this IQueryable<FreelancerProfileEntity> query)
        {
            return query
                .Include(x => x.User)
                    .ThenInclude(x => x.Avatar)
                .Include(x => x.PortfolioProjects)
                .Include(x => x.Reviews)
                .Include(x => x.Skills);
        }
    }
}
