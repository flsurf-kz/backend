using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetWorkSessionQuery : BaseQuery
    {
        public Guid WorkSessionId { get; set; }
    }

    public class GetWorkSessionHandler : IQueryHandler<GetWorkSessionQuery, WorkSessionEntity?>
    {
        private readonly IApplicationDbContext _context;

        public GetWorkSessionHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkSessionEntity?> Handle(GetWorkSessionQuery query)
        {
            var session = await _context.WorkSessions
                .Include(x => x.Files)
                .Include(x => x.Freelancer)
                    .ThenInclude(x => x.Avatar)
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(ws => ws.Id == query.WorkSessionId);
            return session;
        }
    }
}
