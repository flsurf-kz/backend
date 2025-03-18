using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Queries
{
    public class GetTransactionProvidersQuery : BaseQuery
    {
        public bool Available { get; set; } = true;
    }

    public class GetTransactionProviders
        : IQueryHandler<GetTransactionProvidersQuery, ICollection<TransactionProviderEntity>>
    {
        private IApplicationDbContext _context;

        public GetTransactionProviders(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<ICollection<TransactionProviderEntity>> Handle(
            GetTransactionProvidersQuery dto)
        {
            return await _context.TransactionProviders
                .Include(x => x.Systems)
                .Include(x => x.Logo)
                .Where(x => x.)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
