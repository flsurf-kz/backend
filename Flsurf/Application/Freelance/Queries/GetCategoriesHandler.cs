using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetCategoriesHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetCategoriesQuery, List<CategoryEntity>>
    {
        private IApplicationDbContext _dbContext = dbContext;

        public async Task<List<CategoryEntity>> Handle(GetCategoriesQuery query)
        {
            var categoriesQuery = _dbContext.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchQuery))
            {
                categoriesQuery = categoriesQuery.Where(c => c.Name.Contains(query.SearchQuery));
            }

            var categories = await categoriesQuery
                .Skip(query.Start)
                .Take(query.Ends - query.Start)
                .ToListAsync();

            return categories;
        }
    }

}
