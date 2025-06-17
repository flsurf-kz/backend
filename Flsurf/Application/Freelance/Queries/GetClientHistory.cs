using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Queries
{
    public class ClientHistoryDto
    {
        public Guid ContractId { get; init; }
        public string JobTitle { get; init; } = string.Empty;
        public DateTime CompletedAt { get; init; }
        public string FreelancerName { get; init; } = string.Empty;
        public FileEntity? FreelancerAvatar { get; init; }
        public decimal AmountPaid { get; init; }
    }

    public class GetClientHistoryQuery : BaseQuery
    {
        /// <summary>
        /// Pagination: [skip, take]
        /// </summary>
        public int[]? RangeOfJobs { get; set; }

        /// <summary>
        /// Фильтр по конкретному фрилансеру
        /// </summary>
        public Guid? FreelancerId { get; set; }

        /// <summary>
        /// Фильтр по диапазону суммы выплат: [min, max]
        /// </summary>
        public decimal[]? PaidToFreelancer { get; set; }

        /// <summary>
        /// Фильтр по дате завершения контрактов
        /// </summary>
        public DateTime? CompletedAfter { get; set; }
        public DateTime? CompletedBefore { get; set; }
    }

    public class GetClientHistoryHandler
        : IQueryHandler<GetClientHistoryQuery, ICollection<ClientHistoryDto>>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPermissionService _perm;

        public GetClientHistoryHandler(
            IApplicationDbContext dbContext,
            IPermissionService permService)
        {
            _db = dbContext;
            _perm = permService;
        }

        public async Task<ICollection<ClientHistoryDto>> Handle(GetClientHistoryQuery query)
        {
            // 1. Получаем текущего клиента
            var client = await _perm.GetCurrentUser();
            var clientId = client.Id;

            // 2. Базовый запрос: завершённые контракты этого клиента
            var qc = _db.Contracts
                .Include(c => c.Job)
                .Include(c => c.Freelancer)
                .Where(c => c.EmployerId == clientId
                         && c.Status == ContractStatus.Completed);

            // 3. Применяем фильтры
            if (query.FreelancerId.HasValue)
                qc = qc.Where(c => c.FreelancerId == query.FreelancerId.Value);

            if (query.PaidToFreelancer != null && query.PaidToFreelancer.Length == 2)
            {
                var min = query.PaidToFreelancer[0];
                var max = query.PaidToFreelancer[1];
                qc = qc.Where(c =>
                    (c.BudgetType == BudgetType.Fixed
                        ? c.Budget.Amount
                        : c.CostPerHour.Amount * c.WorkSessions
                            .Where(ws => ws.EndDate.HasValue)
                            .Sum(ws => (decimal)(ws.EndDate!.Value - ws.StartDate).TotalHours))
                    >= min
                    &&
                    (c.BudgetType == BudgetType.Fixed
                        ? c.Budget.Amount
                        : c.CostPerHour.Amount * c.WorkSessions
                            .Where(ws => ws.EndDate.HasValue)
                            .Sum(ws => (decimal)(ws.EndDate!.Value - ws.StartDate).TotalHours))
                    <= max
                );
            }

            if (query.CompletedAfter.HasValue)
                qc = qc.Where(c => c.EndDate >= query.CompletedAfter.Value);

            if (query.CompletedBefore.HasValue)
                qc = qc.Where(c => c.EndDate <= query.CompletedBefore.Value);

            // 4. Сортировка по дате завершения
            qc = qc.OrderByDescending(c => c.EndDate);

            // 5. Пагинация
            if (query.RangeOfJobs != null && query.RangeOfJobs.Length == 2)
            {
                var skip = query.RangeOfJobs[0];
                var take = query.RangeOfJobs[1];
                qc = qc.Skip(skip).Take(take);
            }

            // 6. Проекция в DTO
            var list = await qc.Select(c => new ClientHistoryDto
            {
                ContractId = c.Id,
                JobTitle = c.Job.Title,
                CompletedAt = c.EndDate!.Value,
                FreelancerName = c.Freelancer.Fullname,
                FreelancerAvatar = c.Freelancer.Avatar,
                AmountPaid = c.BudgetType == BudgetType.Fixed
                                      ? c.Budget.Amount
                                      : c.CostPerHour.Amount *
                                        c.WorkSessions
                                          .Sum(ws => (decimal)(ws.EndDate!.Value - ws.StartDate).TotalHours)
            })
            .ToListAsync();

            return list;
        }
    }
}
