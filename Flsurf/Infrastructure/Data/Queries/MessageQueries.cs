using Flsurf.Domain.Messanging.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class MessageQueries
    {
        public static IQueryable<MessageEntity> IncludeStandard(this IQueryable<MessageEntity> query)
        {
            return query
                .Include(x => x.Chat)
                .Include(x => x.Files)
                .Include(x => x.Sender); 
        }
    }
}
