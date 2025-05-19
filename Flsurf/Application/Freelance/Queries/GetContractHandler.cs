using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContractHandler(IPermissionService permService, IApplicationDbContext dbContext)
        : IQueryHandler<GetContractQuery, ContractEntity?>
    {
        private IPermissionService permissionService = permService;
        private IApplicationDbContext _dbContext = dbContext;

        public async Task<ContractEntity?> Handle(GetContractQuery query)
        {
            var userId = (await permissionService.GetCurrentUser()).Id;
            var rel = ZedFreelancerUser.WithId(userId).CanReadContract(ZedContract.WithId(query.ContractId)); 
            await permissionService.EnforceCheckPermission(rel);

            var contract = await _dbContext.Contracts
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == query.ContractId);

            return contract;
        }
    }

}
