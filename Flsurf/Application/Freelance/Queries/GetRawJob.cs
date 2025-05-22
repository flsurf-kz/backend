using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetRawJobQuery : BaseQuery { 
        public Guid JobId { get; set; }
    }
    
    public class GetRawJobHandler(IApplicationDbContext dbContext) : IQueryHandler<GetRawJobQuery, JobEntity?>
    {
        public async Task<JobEntity?> Handle(GetRawJobQuery query)
        {
            var job = await dbContext.Jobs
                .IncludeStandard()
                
                .FirstOrDefaultAsync(x => x.Id == query.JobId);
            return job; 
        }
    }
}
