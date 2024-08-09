using Flsurf.Domain.Messanging.Entities;
using Flsurf.Domain.Payment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class ChatQueries
    {
        public static IQueryable<ChatEntity> IncludeStandard(this IQueryable<ChatEntity> query)
        {
            return query
                .Include(x => x.Participants)
                .Include(x => x.Owner)
                .Include(x => x.Contracts); 
        }
    }
}
