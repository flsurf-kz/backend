using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class TransactionQueries
    {
        public static IQueryable<TransactionEntity> IncludeStandard(this IQueryable<TransactionEntity> query)
        {
            return query
                .Include(x => x.CreatedByUser)
                .Include(x => x.Provider); 
        }

        public static IQueryable<TransactionEntity> FilterByParams(this IQueryable<TransactionEntity> query,
            DateTime? fromDate, DateTime? toDate, TransactionType? operation, string? providerName)
        {
            if (fromDate != null && toDate != null)
            {
                query = query
                    .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);
            }
            if (operation != null)
            {
                query = query.Where(x => x.Operation == operation);
            }
            if (providerName != null)
            {
                query = query.Where(x => x.Provider.Name == providerName);
            }
            return query;
        }
    }
}
