﻿using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class GetTransactionsList : BaseUseCase<GetTransactionsListDto, ICollection<TransactionEntity>>
    {
        private IApplicationDbContext _context;
        private IUser _user;
        private IPermissionService _permService;

        public GetTransactionsList(IApplicationDbContext dbContext, IUser user, IPermissionService permService)
        {
            _user = user;
            _context = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<TransactionEntity>> Execute(GetTransactionsListDto dto)
        {
            var wallet = await _context.Wallets
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == dto.WalletId);

            Guard.Against.Null(wallet, message: "Wallet does not exists");

            var owner = await _permService.GetCurrentUser(); 

            await _permService.EnforceCheckPermission(
                ZedPaymentUser
                    .WithId(owner.Id)
                    .CanReadWallet(ZedWallet.WithId(dto.WalletId)));

            var result = await _context.Transactions
                .IncludeStandard()
                .FilterByParams(
                    fromDate: dto.FromDate,
                    toDate: dto.ToDate,
                    operation: dto.Operation,
                    providerName: dto.TransactionProvider)
                .ToListAsync();

            return result;
        }
    }
}
