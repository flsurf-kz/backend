using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Study.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class StudentQueries
    {
        public static IQueryable<StudentEntity> IncludeStandard(this IQueryable<StudentEntity> query)
        {
            return query
                .Include(x => x.User)
                .Include(x => x.Courses)
                .Include(x => x.InstitutionMember); 
        }
    }
}
