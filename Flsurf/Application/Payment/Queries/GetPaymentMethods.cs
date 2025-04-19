using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Queries.Models;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Queries
{
    public class GetPaymentMethodsQuery : BaseQuery
    {
        public Guid UserId { get; set; }
    }

    public class GetPaymentMethodsHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetPaymentMethodsQuery, List<PaymentMethodDto>>
    {

        public async Task<List<PaymentMethodDto>> Handle(
            GetPaymentMethodsQuery req)
        {
            return await dbContext.PaymentMethods
                .AsNoTracking()
                .Where(pm => pm.UserId == req.UserId && pm.IsActive)
                .Select(pm => new PaymentMethodDto(
                    pm.Id,
                    pm.ProviderId,
                    pm.Brand,
                    $"•••• •••• •••• {pm.Last4}",    // mask
                    pm.ExpMonth,
                    pm.ExpYear,
                    pm.IsDefault))
                .ToListAsync();
        }
    }
}
