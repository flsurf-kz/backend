using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class BalanceOperation : BaseUseCase<BalanceOperationDto, bool>
    {
        private IApplicationDbContext _context;
        private IPermissionService _permService;

        public BalanceOperation(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<bool> Execute(BalanceOperationDto dto)
        {
            var owner = await _permService.GetCurrentUser();
            await _permService.EnforceCheckPermission(
                ZedPaymentUser
                    .WithId(owner.Id)
                    .CanAddBalance(ZedWallet.WithId(dto.WalletId)));

            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == dto.WalletId);

            Guard.Against.Null(wallet, message: "Wallet does not exists");

            wallet.AddBalance(dto.Balance);

            return true;
        }
    }
}
