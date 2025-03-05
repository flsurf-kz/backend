using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetContractsListHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : IQueryHandler<GetContractsListQuery, List<ContractEntity>>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<List<ContractEntity>> Handle(GetContractsListQuery query)
        {
            var userId = query.UserId ?? (await _permService.GetCurrentUser()).Id; 

            // 🔥 Получаем контракты, к которым пользователь имеет доступ
            var perms = _permService.LookupSubjects(
                ZedFreelancerUser.WithId(userId),
                "read",
                "contract");

            List<Guid> contractIds = [];
            Guid contractId;

            await foreach (var perm in perms)
            {
                if (Guid.TryParse(perm.Subject.Id, out contractId))
                {
                    contractIds.Add(contractId);
                }
            }

            if (!contractIds.Any())
                return [];

            var contractsQuery = _dbContext.Contracts
                .IncludeStandard() // Загружаем все необходимые данные
                .Where(c => contractIds.Contains(c.Id)) // Фильтруем по доступным контрактам
                .OrderByDescending(c => c.CreatedAt) // Новые контракты первыми
                .Skip(query.Start)
                .Take(query.Ends);

            return await contractsQuery.ToListAsync();
        }
    }

}
