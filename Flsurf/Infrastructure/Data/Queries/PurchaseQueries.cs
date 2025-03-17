using Flsurf.Domain.Payment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Queries
{
    public static class PurchaseQueries
    {
        public static IQueryable<PurchaseEntity> IncludeStandard(this IQueryable<PurchaseEntity> query)
        {
            return query
                .Include(x => x.Transaction)
                .Include(x => x.Transaction)
                //.Include(x => x.Product)
                //    .ThenInclude(x => x.Images)
                //.Include(x => x.Product)
                //    .ThenInclude(x => x.Game)
                //        .ThenInclude(x => x.Logo)
                //.Include(x => x.Product)
                //    .ThenInclude(x => x.Category)
                //.Include(x => x.Product)
                //    .ThenInclude(x => x.CreatedBy)
                .Include(x => x.Wallet);
        }
    }
}
