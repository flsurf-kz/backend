using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerProfileListHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetFreelancerProfileListQuery, List<FreelancerProfileEntity>>
    {
        private IApplicationDbContext _dbContext = dbContext;

        public async Task<List<FreelancerProfileEntity>> Handle(GetFreelancerProfileListQuery query)
        {
            var freelancersQuery = _dbContext.FreelancerProfiles.AsQueryable();

            if (query.Skills is not null && query.Skills.Length > 0)
            {
                freelancersQuery = freelancersQuery.Where(f => query.Skills.All(skill => f.Skills.Contains(skill)));
            }

            if (query.CostPerHour is not null && query.CostPerHour.Length == 2)
            {
                freelancersQuery = freelancersQuery.Where(f => f.CostPerHour >= query.CostPerHour[0]
                                                             && f.CostPerHour <= query.CostPerHour[1]);
            }

            if (query.ReviewsCount is not null && query.ReviewsCount.Length == 2)
            {
                freelancersQuery = freelancersQuery.Where(f => f.Reviews.Count >= query.ReviewsCount[0]
                                                             && f.Reviews.Count <= query.ReviewsCount[1]);
            }

            var freelancers = await freelancersQuery
                .OrderByDescending(f => f.Rating) // сортируем по рейтингу
                .Paginate(query.Start, query.Ends)
                .Where(x => x.IsHidden == false)
                .IncludeStandard()
                .ToListAsync();  // very heavy weight 

            return freelancers;
        }
    }

}
