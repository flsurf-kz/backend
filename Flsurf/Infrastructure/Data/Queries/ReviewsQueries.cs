using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class ReviewsQueries
    {
        public static IQueryable<ReviewEntity> IncludeStandard(this IQueryable<ReviewEntity> query)
        {
            return query
                .Include(x => x)
        }
    }
}
