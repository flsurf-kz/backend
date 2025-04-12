using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Queries.Responses;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetSkillsHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetSkillsQuery, List<SkillModel>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<List<SkillModel>> Handle(GetSkillsQuery query)
        {
            var skillsQuery = _dbContext.Skills.AsQueryable();

            // 🔎 Поиск по названию навыка
            if (!string.IsNullOrWhiteSpace(query.SearchQuery))
            {
                string searchLower = query.SearchQuery.ToLower();
                skillsQuery = skillsQuery.Where(s => s.Name.ToLower().Contains(searchLower));
            }

            // 🔥 Пагинация + сортировка
            var skills = await skillsQuery
                .OrderBy(s => s.Name)
                .Paginate(query.Starts, query.Ends)
                .Select(x => new SkillModel
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();

            return skills;
        }
    }

}
