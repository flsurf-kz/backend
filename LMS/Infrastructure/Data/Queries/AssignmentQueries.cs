using Flsurf.Domain.Study.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class AssignmentQueries
    {
        public static IQueryable<AssignmentEntity> IncludeStandard(this IQueryable<AssignmentEntity> query)
        {
            return query
                .Include(x => x.Attachments)
                .Include(x => x.AssignedBy)
                .Include(x => x.GradeType);
        }
    }
}
