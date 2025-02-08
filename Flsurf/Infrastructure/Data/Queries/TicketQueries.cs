using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class TicketQueries
    {
        public static IQueryable<TicketEntity> IncludeStandard(this IQueryable<TicketEntity> query)
        {
            return query
                .Include(x => x.AssignedUser)
                .Include(x => x.CreatedBy)
                .Include(x => x.Files)
                .Include(x => x.Subject)
                .Include(x => x.Comments); 
        }
    }
}
