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
        : IQueryHandler<GetWorkSnapshotsListQuery, List<WorkSessionEntity>>
    {
        private readonly IPermissionService _permService = permService;
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<List<WorkSessionEntity>> Handle(GetWorkSnapshotsListQuery query)
        {
            // 🔐 Получаем текущего пользователя
            var user = await _permService.GetCurrentUser();

            // 🔎 Проверяем, может ли пользователь читать снэпшоты работы
            var hasPermission = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanReadContract(ZedContract.WithId(query.ContractId)));

            if (!hasPermission) throw new AccessDenied("User has no access to work snapshots");

            // 🔥 Получаем снэпшоты работы
            var snapshotsQuery = _dbContext.WorkSessions
                .Where(ws => ws.ContractId == query.ContractId) // Фильтр по контракту
                .OrderByDescending(ws => ws.CreatedAt); // Сортировка по дате создания (новые сначала)

            // 🔥 Пагинация
            var snapshots = await snapshotsQuery
                .Paginate(query.Start, query.Ends)
                .ToListAsync();

            return snapshots;
        }
    }

}
