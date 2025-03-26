using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContestQuery : BaseQuery
    {
        public Guid ContestId { get; set; }
    }

    public class GetContestHandler : IQueryHandler<GetContestQuery, ContestEntity?>
    {
        private readonly IApplicationDbContext _context;

        public GetContestHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContestEntity?> Handle(GetContestQuery query)
        {
            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.Id == query.ContestId);
            return contest;
        }
    }
}
