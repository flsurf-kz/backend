using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContractsListHandler(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        : IQueryHandler<GetContractsListQuery, List<ContractEntity>>
    {
        private readonly IApplicationDbContext _db = dbContext;
        private readonly IPermissionService _ps = permService;

        public async Task<List<ContractEntity>> Handle(GetContractsListQuery q)
        {
            /* ---- текущий пользователь ------------------------------------ */
            Guid userId = q.UserId ?? (await _ps.GetCurrentUser()).Id;

            /* ---- 1. ReBAC: контракт-иды, на которые есть право read ------ */
            HashSet<Guid> allowed = new();

            await foreach (var rel in _ps.LookupSubjects(
                               ZedFreelancerUser.WithId(userId),
                               "read", "contract"))
            {
                if (Guid.TryParse(rel.Subject.Id, out var cid))
                    allowed.Add(cid);
            }

            /* ---- 2. Прямые отношения Employer / Freelancer --------------- */
            IQueryable<ContractEntity> baseQuery = _db.Contracts
                                                      .IncludeStandard()
                                                      .Where(c =>
                                                             c.EmployerId == userId ||
                                                             c.FreelancerId == userId);

            /* ---- 3. Пагинация, сортировка -------------------------------- */
            var list = await baseQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip(q.Start)
                .Take(q.Ends - q.Start)
                .ToListAsync();

            return list;
        }
    }

}
