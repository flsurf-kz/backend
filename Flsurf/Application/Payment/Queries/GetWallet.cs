using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Queries
{
    public class GetWalletQuery : BaseQuery
    {
        public Guid? WalletId { get; set; }
        public Guid? UserId { get; set; }
    }

    public class GetWallet : IQueryHandler<GetWalletQuery, WalletEntity?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetWallet(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<WalletEntity?> Handle(GetWalletQuery dto)
        {
            var wallet = await _context.Wallets
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == dto.WalletId || x.UserId == dto.UserId);

            if (wallet == null)
                return wallet; 

            await _permService.EnforceCheckPermission(
                ZedPaymentUser
                    .WithId(wallet.UserId)
                    .CanReadWallet(ZedWallet.WithId(wallet.Id)));

            return wallet;
        }
    }
}
