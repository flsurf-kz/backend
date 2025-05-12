using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Queries.Responses;
using Hangfire.Storage.Monitoring;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetJobHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetJobQuery, JobDetails?>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<JobDetails?> Handle(GetJobQuery query)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Employer) // Загружаем клиента
                .Include(x => x.Category)
                .Include(x => x.RequiredSkills)
                .Where(j => j.Id == query.JobId)
                .AsNoTracking()
                .Select(j => new JobDetails
                {
                    JobId = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Status = j.Status,
                    Budget = j.Payout,
                    // TODO 
                    Category = j.Category.Name,
                    CategorySlug = j.Category.Slug, 
                    Skills = j.RequiredSkills.Select(x => x.Name).ToArray(),
                    //Languages = j.Languages.Split(','),

                    CreatedAt = j.CreatedAt ?? DateTime.UtcNow,

                    // 🔥 **Активность на заказе**
                    ResponsesRangeMin = 20, // Можно вынести в конфиг
                    ResponsesRangeMax = 30,
                    DailyResponsesMin = 5,
                    DailyResponsesMax = 10,
                    ConfirmedResponses = 5, 
                    Views = 10, 
                })
                .FirstOrDefaultAsync();

            return job;
        }
    }

}
