using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Staff.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class JobReviewsQueries
    {
        public static IQueryable<JobReviewEntity> IncludeStandard(this IQueryable<JobReviewEntity> query)
        {
            return query
                .Include(x => x.Job)
                .Include(x => x.Reviewer)
                .Include(x => x.Target); 
        }
    }
}
