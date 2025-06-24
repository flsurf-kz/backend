using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Application.Payment.Queries.Models;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Queries
{
    public class GetPaymentMethodsQuery : BaseQuery
    {
        public Guid? UserId { get; set; }
    }

    public class GetPaymentMethodsHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : IQueryHandler<GetPaymentMethodsQuery, List<PaymentMethodDto>>
    {

        public async Task<List<PaymentMethodDto>> Handle(
            GetPaymentMethodsQuery req)
        {
            var currentUser = await permService.GetCurrentUser(); 
            Guid userId;
            if (req.UserId == null)
                userId = currentUser.Id; 
            else if (req.UserId != currentUser.Id)
            {
                var wallet = await dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == req.UserId);
                if (wallet == null)
                    throw new AccessDenied("NO");

                var ok = await permService.CheckPermission(ZedPaymentUser
                    .WithId(currentUser.Id)
                    .CanReadWallet(ZedWallet.WithId(wallet.Id)));
                if (!ok)
                    throw new AccessDenied("NOT OK");
                userId = wallet.UserId;
            }
            else { userId = Guid.Empty; }
            

            return await dbContext.PaymentMethods
                .Where(pm => pm.UserId == (req.UserId ?? userId) && pm.IsActive)
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
