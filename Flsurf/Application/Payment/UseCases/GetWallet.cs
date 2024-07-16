using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class GetWallet : BaseUseCase<GetWalletDto, WalletEntity>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccessPolicy _accessPolicy;

        public GetWallet(IApplicationDbContext dbContext, IAccessPolicy accessPolicy)
        {
            _context = dbContext;
            _accessPolicy = accessPolicy;
        }

        public async Task<WalletEntity> Execute(GetWalletDto dto)
        {
            var wallet = await _context.Wallets
                .AsNoTracking()
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == dto.WalletId || x.UserId == dto.UserId);

            Guard.Against.Null(wallet, message: "Wallet does not exists");

            await _accessPolicy.EnforceRelationship(PermissionEnum.read, wallet, wallet.User.Id);

            return wallet;
        }
    }
}
