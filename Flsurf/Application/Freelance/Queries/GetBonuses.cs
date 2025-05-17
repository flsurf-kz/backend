// --- Flsurf.Application.Freelance.Queries.GetBonusesForContractQuery.cs ---
using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // For ICollection
using System.Linq; // For Where

namespace Flsurf.Application.Freelance.Queries
{
    public class GetBonusesForContractQuery : BaseQuery
    {
        public Guid ContractId { get; set; }
    }

    public class GetBonusesForContractHandler : IQueryHandler<GetBonusesForContractQuery, ICollection<BonusEntity>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public GetBonusesForContractHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<ICollection<BonusEntity>> Handle(GetBonusesForContractQuery query)
        {
            var currentUser = await _permService.GetCurrentUser();
            if (currentUser == null)
            {
                // Or throw, or return empty list based on how unauthorized queries are handled
                return new List<BonusEntity>();
            }

            var contract = await _dbContext.Contracts
                // Select only needed fields for the permission check if performance is critical
                // .Select(c => new { c.Id, c.EmployerId, c.FreelancerId })
                .FirstOrDefaultAsync(c => c.Id == query.ContractId);

            if (contract == null)
            {
                // Contract not found, so no bonuses to return, or could throw specific NotFoundException
                return new List<BonusEntity>();
            }

            // Permission Check: User must be the employer or freelancer on the contract
            if (currentUser.Id != contract.EmployerId && currentUser.Id != contract.FreelancerId)
            {
                // Not authorized to view bonuses for this contract.
                // Following GetContractHandler example, you might use EnforceCheckPermission
                // For now, returning an empty list as per "just check EmployerId, and FreelancerId"
                // var rel = ZedUser.WithId(currentUser.Id).CanReadBonusesForContract(ZedContract.WithId(query.ContractId));
                // try { await _permService.EnforceCheckPermission(rel); } catch { return new List<BonusEntity>(); }
                return new List<BonusEntity>();
            }

            var bonuses = await _dbContext.Bonuses
                .Where(b => b.ContractId == query.ContractId)
                .OrderByDescending(b => b.CreatedAt) // Optional: order them, e.g., by creation date
                .ToListAsync();

            return bonuses;
        }
    }
}