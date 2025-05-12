using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetProposalsListQuery : BaseQuery
    {
        public Guid? JobId { get; set; }
        public ProposalStatus Status { get; set; }
    }

    public class GetProposalsListHandler(IApplicationDbContext dbContext) : IQueryHandler<GetProposalsListQuery, ICollection<ProposalEntity>>
    {
        public async Task<ICollection<ProposalEntity>> Handle(GetProposalsListQuery queryDto)
        {
            var query = dbContext.Proposals
                .Include(x => x.Freelancer)
                .Include(x => x.Job)
                .Include(x => x.Files)
                .Where(x => x.Status == queryDto.Status)
                .AsQueryable(); 

            if (queryDto.JobId != null)
            {
                query = query.Where(x => x.JobId == queryDto.JobId);
            }

            return await query.ToListAsync(); 
        }
    }
}
