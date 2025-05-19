using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class ContractQueries
    {
        public static IQueryable<ContractEntity> IncludeStandard(this IQueryable<ContractEntity> query)
        {
            return query
                .Include(x => x.Bonuses)
                .Include(x => x.Tasks)
                .Include(x => x.WorkSessions)
                .Include(x => x.Job)
                .Include(c => c.Employer)
                    .ThenInclude(x => x.Avatar)
                .Include(c => c.Freelancer)
                    .ThenInclude(x => x.Avatar);
        }
    }

}
