using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContestListQuery : BaseQuery
    {
        public int Start { get; set; } = 0;
        public int Ends { get; set; } = 10; 
    }

    public class GetContestListHandler : IQueryHandler<GetContestListQuery, ICollection<ContestEntity>>
    {
        private readonly IApplicationDbContext _context;

        public GetContestListHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<ContestEntity>> Handle(GetContestListQuery query)
        {
            int count = query.Ends - query.Start;
            return await _context.Contests
                .Skip(query.Start)
                .Take(count)
                .ToListAsync();
        }
    }
}
