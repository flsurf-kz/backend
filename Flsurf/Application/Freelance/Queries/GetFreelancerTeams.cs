using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancerTeamsListQuery : BaseQuery
    {
        public int Start { get; } = 0;
        public int Ends { get; } = 10; 
    }

    public class GetFreelancerTeamsHandler(
        IApplicationDbContext _dbContext, 
        IPermissionService _permService
    ) : IQueryHandler<GetFreelancerTeamsListQuery, List<FreelancerTeamEntity>>
    {
        private IApplicationDbContext dbContext = _dbContext; 
        private IPermissionService permissionService = _permService;

        public async Task<List<FreelancerTeamEntity>> Handle(GetFreelancerTeamsListQuery query)
        {
            var user = await permissionService.GetCurrentUser(); 

            // should return flsurf freelancer teams ids 
            var subjects = permissionService.LookupSubjects(ZedFreelancerUser.WithId(user.Id), "read", "flsurf/team");

            List<Guid> ids = []; 

            await foreach (var subject in subjects)
            {
                ids.Add(Guid.Parse(subject.Subject.Id)); 
            }

            var teams = await dbContext.FreelancerTeams
                .IncludeStandard()
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return teams; 
        }
    }
}
