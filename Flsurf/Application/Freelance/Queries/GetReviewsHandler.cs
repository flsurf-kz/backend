using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetReviewsHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetReviewsQuery, List<JobReviewEntity>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<List<JobReviewEntity>> Handle(GetReviewsQuery query)
        {
            var reviewsQuery = _dbContext.Reviews
                .Where(r => r.TargetId == query.UserId) // Фильтр по пользователю
                .OrderByDescending(r => r.CreatedAt); // Сортировка по дате

            // 🔥 Пагинация
            var reviews = await reviewsQuery
                .IncludeStandard()
                .Paginate(query.Starts, query.Ends)
                .ToListAsync();

            return reviews;
        }
    }

}
