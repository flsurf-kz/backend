using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class FreelancerTeamQueries
    {
        public static IQueryable<FreelancerTeamEntity> IncludeStandard(this IQueryable<FreelancerTeamEntity> query)
        {
            return query
                .Include(c => c.Participants)
                .Include(c => c.Owner)
                .Include(c => c.Invitations)
                .Include(c => c.Avatar); 
        }
    }
}
