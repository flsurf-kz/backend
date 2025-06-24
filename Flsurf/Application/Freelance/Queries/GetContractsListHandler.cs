using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContractsListHandler(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        : IQueryHandler<GetContractsListQuery, List<ContractEntity>>
    {

        public async Task<List<ContractEntity>> Handle(GetContractsListQuery q)
        {
            /* ---- текущий пользователь ------------------------------------ */
            Guid userId = q.UserId ?? (await permService.GetCurrentUser()).Id;

            /* ---- 1. ReBAC: контракт-иды, на которые есть право read ------ */
            HashSet<Guid> allowed = new();

            await foreach (var rel in permService.LookupSubjects(
                               ZedFreelancerUser.WithId(userId),
                               "read", "contract"))
            {
                if (Guid.TryParse(rel.Subject.Id, out var cid))
                    allowed.Add(cid);
            }

            /* ---- 2. Прямые отношения Employer / Freelancer --------------- */
            IQueryable<ContractEntity> baseQuery = dbContext.Contracts
                                                      .IncludeStandard()
                                                      .Where(c =>
                                                             c.EmployerId == userId ||
                                                             c.FreelancerId == userId);

            if (q.InDispute == true)                       // ищем ТОЛЬКО спорные
            {
                baseQuery = baseQuery.Where(x => x.IsPaused &&
                                                 x.Status == ContractStatus.Paused);
            }
            else if (q.InDispute == false)                 // явно НЕ хотим спорные
            {
                baseQuery = baseQuery.Where(x => !x.IsPaused ||
                                                 x.Status != ContractStatus.Paused);
            }
            /* ---- 3. Пагинация, сортировка -------------------------------- */
            var list = await baseQuery
                .OrderByDescending(c => c.CreatedAt)
                .Paginate(q.Start, q.Ends)
                .ToListAsync();

            return list;
        }
    }

}
