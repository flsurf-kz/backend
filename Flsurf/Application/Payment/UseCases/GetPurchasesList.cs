using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class GetPurchasesList : BaseUseCase<GetPurchasesListDto, ICollection<PurchaseEntity>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public GetPurchasesList(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _context = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<PurchaseEntity>> Execute(GetPurchasesListDto dto)
        {
            var query = _context.Purchases
                .IncludeStandard()
                .AsQueryable();

            if (!await _permService.CheckPermission(PermissionEnum.read, "*") && dto.UserId != null)
            {
                throw new AccessDenied(null);
            }
            if (dto.UserId != null)
            {
                query = query.Where(x => x.Transaction.CreatedByUser.Id == dto.UserId);
            }
            //if (dto.SoldByUserId != null)
            //{
            //    query = query.Where(x => x.Product.CreatedById == dto.SoldByUserId);
            //}
            if (dto.StartTime != null && dto.EndTime != null)
            {
                query = query
                    .Where(x => x.CreatedAt <= dto.StartTime && x.CreatedAt >= dto.EndTime);
            }
            if (dto.Operation != null)
            {
                query = query
                    .Where(x => x.Transaction.Operation == dto.Operation);
            }
            if (dto.Direction != null)
            {
                query = query
                    .Where(x => x.Transaction.Direction == dto.Direction);
            }
            if (dto.Status != null)
            {
                query = query
                    .Where(x => x.Status == dto.Status);
            }
            if (dto.TransactionSatus != null)
            {
                query = query
                    .Where(x => x.Transaction.Status == dto.TransactionSatus);
            }
            query = query.Paginate(dto.Start, dto.Ends);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
