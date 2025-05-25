using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Queries.Responses;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetSkillQuery : BaseQuery {  
        public Guid SkillId { get; set; }
    }

    public class GetSkillHandler(IApplicationDbContext dbContext) : IQueryHandler<GetSkillQuery, SkillModel?> 
    {
        public async Task<SkillModel?> Handle(GetSkillQuery query)
        {
            return await dbContext.Skills
                .Select(x => new SkillModel() { Id = x.Id, Name = x.Name })
                .FirstOrDefaultAsync(x => x.Id == query.SkillId);
        }
    }
}
