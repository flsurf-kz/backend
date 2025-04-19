using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class TransactionQueries
    {
        public static IQueryable<TransactionEntity> IncludeStandard(this IQueryable<TransactionEntity> query)
        {
            return query; 
        }

        public static IQueryable<TransactionEntity> FilterByParams(this IQueryable<TransactionEntity> query,
            DateTime? fromDate, DateTime? toDate, TransactionType? operation, string? providerName, 
            decimal[]? pricesRange, TransactionFlow? flow, TransactionStatus? status)
        {
            if (fromDate != null && toDate != null)
            {
                query = query
                    .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);
            }
            if (operation != null)
            {
                query = query.Where(x => x.Type == operation);
            }
            if (pricesRange != null && pricesRange.Length > 2)
            {
                query = query.Where(x => pricesRange[1] > x.NetAmount.Amount && x.NetAmount.Amount > pricesRange[0]); 
            }
            if (providerName != null && string.IsNullOrWhiteSpace(providerName))
            {
                query = query.Where(x => x.Props != null && x.Props.PaymentGateway == providerName);
            }
            if (flow != null)
            {
                query = query.Where(x => x.Flow == flow); 
            }
            if (status != null)
            {
                query = query.Where(x => x.Status == status); 
            }
            return query;
        }
    }
}
