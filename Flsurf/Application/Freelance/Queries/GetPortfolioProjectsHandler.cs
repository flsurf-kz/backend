using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetPortfolioProjectsHandler(IApplicationDbContext dbContext, IPermissionService permService)
    : IQueryHandler<GetPortfolioProjectsQuery, List<PortfolioProjectEntity>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<List<PortfolioProjectEntity>> Handle(GetPortfolioProjectsQuery query)
        {
            // 🔥 Получаем текущего пользователя
            var currentUser = await _permService.GetCurrentUser();
            bool isOwner = currentUser.Id == query.FreelancerId;

            // 🔥 Запрос проектов (общедоступные + черновики, если владелец)
            var projectsQuery = _dbContext.PortfolioProjects
                .Where(p => p.UserId == query.FreelancerId &&
                            (p.Hidden != false || isOwner))
                .OrderByDescending(p => p.CreatedAt)
                .Skip(query.Start)
                .Take(query.Ends);

            return await projectsQuery.ToListAsync();
        }
    }

}
