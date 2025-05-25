using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetCategoryQuery : BaseQuery
    {
        public Guid CategoryId { get; set; }
    }

    public class GetCategoryHandler(IApplicationDbContext dbContext): IQueryHandler<GetCategoryQuery, CategoryEntity?>
    {
        public async Task<CategoryEntity?> Handle(GetCategoryQuery query)
        {
            return await dbContext.Categories
                .Include(x => x.SubCategories)
                .Include(x => x.ParentCategory)
                .Where(x => x.Id == query.CategoryId)
                .FirstOrDefaultAsync(); 
        }
    }
}
