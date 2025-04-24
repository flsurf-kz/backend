using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Queries.Responses;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerStatsQuery : BaseQuery
    {
        public Guid? UserId { get; set; }
    }

    public class GetFreelancerStatsHandler(IApplicationDbContext _dbContext, IPermissionService _permSerivce) : IQueryHandler<GetFreelancerStatsQuery, FreelancerStatsDto>
    {
        public async Task<FreelancerStatsDto> Handle(GetFreelancerStatsQuery query)
        {
            var currentUser = await _permSerivce.GetCurrentUser();
            if (query.UserId == null)
                query.UserId = currentUser.Id;

            if (currentUser.Type != Domain.User.Enums.UserTypes.Freelancer)
                throw new AccessDenied("Пшел отсюда"); 

            // 1) Берем профиль
            var profile = await _dbContext.FreelancerProfiles
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.UserId == query.UserId);

            if (profile == null)
                return null!; // контроллер вернёт 404

            var now = DateTime.UtcNow;
            var oneYearAgo = now.AddYears(-1);

            // 2) Earnings — все WorkSessions за последний год
            var sessions = await _dbContext.WorkSessions
                .Include(ws => ws.Contract)
                .Where(ws =>
                    ws.Contract.FreelancerId == query.UserId &&
                    ws.EndDate.HasValue &&
                    ws.EndDate.Value >= oneYearAgo)
                .ToListAsync();

            decimal earnings = sessions.Sum(ws =>
                (decimal)(ws.EndDate!.Value - ws.StartDate).TotalHours
                * ws.Contract.CostPerHour.Amount);

            // 3) Job Success Score — по среднему рейтингу отзывов (0–5 → 0–100)
            int jss = profile.Reviews.Any()
                ? (int)Math.Round(profile.Reviews.Average(r => r.Rating) * 20)
                : 0;

            // 4) Profile Views — если есть сущность ProfileView (пример)
            var views = await _dbContext.FreelancerProfileViews
                .Where(v =>
                    v.FreelancerId == query.UserId &&
                    v.ViewedAt >= oneYearAgo)
                .GroupBy(v => v.ViewedAt.Date)
                .Select(g => new ProfileViewDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .ToListAsync();

            // 5) Proposals — считаем из ProposalEntity
            var allProposals = await _dbContext.Proposals
                .Where(p => p.FreelancerId == query.UserId)
                .ToListAsync();

            var proposalsDto = new ProposalsDto
            {
                Sent = allProposals.Count,
                Viewed = allProposals.Count,
                Hires = allProposals.Count(p => p.Status == ProposalStatus.Accepted)
            };

            // 6) Клиенты по длительности контрактов
            var contracts = await _dbContext.Contracts
                .Where(c => c.FreelancerId == query.UserId && c.EndDate.HasValue)
                .ToListAsync();

            int longTerm = contracts.Count(c => c.EndDate != null && 
                (c.EndDate.Value - c.StartDate).TotalDays > 90);
            int shortTerm = contracts.Count(c => c.EndDate != null &&
                (c.EndDate.Value - c.StartDate).TotalDays <= 90);

            // Собираем DTO
            return new FreelancerStatsDto
            {
                EarningsLast12Months = earnings,
                JobSuccessScore = jss,
                ProfileViews = views,
                Proposals = proposalsDto,
                LongTermClients = longTerm,
                ShortTermClients = shortTerm
            };
        }
    }
}
