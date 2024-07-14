using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Flsurf.Domain.Payment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class GetTransactionProviders
        : BaseUseCase<GetTransactionProvidersDto, ICollection<TransactionProviderEntity>>
    {
        private IApplicationDbContext _context;

        public GetTransactionProviders(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<ICollection<TransactionProviderEntity>> Execute(
            GetTransactionProvidersDto dto)
        {
            return await _context.TransactionProviders
                .Include(x => x.Systems)
                .Include(x => x.Logo)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
