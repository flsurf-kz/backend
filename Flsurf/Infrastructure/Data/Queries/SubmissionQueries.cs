using Flsurf.Domain.Study.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class SubmissionQueries 
    {
        public static IQueryable<SubmissionEntity> IncludeStandard(this IQueryable<SubmissionEntity> query)
        {
            return query
                .Include(x => x.Attachments); 
        }
    }
}
