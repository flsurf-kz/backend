using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.Queries
{
    public class GetTransactionsListQuery : BaseQuery
    {
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TransactionType? Operation { get; set; }
        public TransactionFlow? Flow { get; set; }
        public string? TransactionProvider { get; set; }
        public Guid? WalletId { get; set; }
        public decimal[]? PriceRange { get; set; }
        public TransactionStatus? Status { get; set; }
        public Guid UserId { get; set; }
    }

    public class GetTransactionsList : IQueryHandler<GetTransactionsListQuery, ICollection<TransactionEntity>>
    {
        private IApplicationDbContext _context;
        private IPermissionService _permService;

        public GetTransactionsList(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<TransactionEntity>> Handle(GetTransactionsListQuery dto)
        {
            Guid ownerId; 
            if (dto.UserId == null)
                ownerId = (await _permService.GetCurrentUser()).Id;
            else 
                ownerId = dto.UserId;
            var query = _context.Wallets
                .Include(x => x.User)
                .AsQueryable();

            if (dto.WalletId == null)
                query = query.Where(x => x.UserId == ownerId);
            else
                query = query.Where(x => x.Id == dto.WalletId); 

            var wallet = await query.FirstOrDefaultAsync();

            if (wallet == null)
                return []; 

            await _permService.EnforceCheckPermission(
                ZedPaymentUser
                    .WithId(ownerId)
                    .CanReadWallet(ZedWallet.WithId(wallet.Id)));

            var result = await _context.Transactions
                .IncludeStandard()
                .FilterByParams(
                    fromDate: dto.FromDate,
                    toDate: dto.ToDate,
                    operation: dto.Operation,
                    providerName: dto.TransactionProvider, 
                    pricesRange: dto.PriceRange, 
                    flow: dto.Flow, 
                    status: dto.Status)
                .Paginate(dto.Start, dto.Ends)
                .ToListAsync();

            return result;
        }
    }
}
