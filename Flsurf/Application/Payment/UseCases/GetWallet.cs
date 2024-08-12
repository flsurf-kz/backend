using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class GetWallet : BaseUseCase<GetWalletDto, WalletEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetWallet(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<WalletEntity> Execute(GetWalletDto dto)
        {
            var wallet = await _context.Wallets
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == dto.WalletId || x.UserId == dto.UserId);

            Guard.Against.Null(wallet, message: "Wallet does not exists");

            await _permService.EnforceCheckPermission(
                ZedPaymentUser
                    .WithId(wallet.UserId)
                    .CanReadWallet(ZedWallet.WithId(wallet.Id)));

            return wallet;
        }
    }
}
