using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetWorkSessionsListHandler(IPermissionService permService, IApplicationDbContext dbContext)
        : IQueryHandler<GetWorkSessionListQuery, List<WorkSessionEntity>>
    {
        private readonly IPermissionService _permService = permService;
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<List<WorkSessionEntity>> Handle(GetWorkSessionListQuery query)
        {
            var user = await _permService.GetCurrentUser();

            var snapshotsQuery = _dbContext.WorkSessions
                .Include(x => x.Files)
                .Include(x => x.Contract)
                .Include(x => x.Freelancer)
                .OrderByDescending(ws => ws.CreatedAt)
                .Paginate(query.Start, query.Ends)
                .AsQueryable();
            if (query.UserId == null && query.ContractId == null)
                query.UserId = user.Id;  // ленивый путь конечо но похер

            if (query.UserId != null && user.Id == query.UserId)
            {
                snapshotsQuery = snapshotsQuery.Where(x => x.FreelancerId == user.Id); 
            } 
            else if (query.UserId != null && user.Id != query.UserId)
            {
                await _permService.EnforceCheckPermission(
                    ZedFreelancerUser.WithId(query.UserId.Value)
                        .CanReadWorkSessions());

                snapshotsQuery = snapshotsQuery.Where(x => x.FreelancerId == query.UserId); 
            }
            else if (query.ContractId != null)
            {
                var hasPermission = await _permService.CheckPermission(
                    ZedFreelancerUser.WithId(user.Id).CanReadContract(ZedContract.WithId((Guid)query.ContractId)));

                if (!hasPermission) throw new AccessDenied("User has no access to work snapshots");

                snapshotsQuery = snapshotsQuery.Where(ws => ws.ContractId == query.ContractId); 
            } 

            return await snapshotsQuery.ToListAsync();
        }
    }

}
