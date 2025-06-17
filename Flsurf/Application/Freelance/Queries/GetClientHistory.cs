using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public string ClientName { get; init; } = string.Empty;
        public FileEntity? ClientAvatar { get; init; }
        public decimal AmountEarned { get; init; }
    }

    public class GetClientHistoryQuery : BaseQuery
    {
        /// <summary>
        /// Pagination: [skip, take]
        /// </summary>
        public int[]? RangeOfContracts { get; set; }

        /// <summary>
        /// Фильтр по конкретному клиенту
        /// </summary>
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Диапазон заработка: [min, max]
        /// </summary>
        public decimal[]? EarnedRange { get; set; }

        /// <summary>
        /// Дата завершения контрактов
        /// </summary>
        public DateTime? CompletedAfter { get; set; }
        public DateTime? CompletedBefore { get; set; }
        public Guid? FreelancerId { get; set; }
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
            // 1. Определяем фрилансера: из запроса или текущего пользователя
            var freelancerId = query.FreelancerId
                ?? (await _perm.GetCurrentUser()).Id;

            // 2. Базовый запрос — завершённые контракты этого фрилансера
            var qc = _db.Contracts
                .Include(c => c.Job)
                .Include(c => c.Employer)
                .Include(c => c.WorkSessions)
                .Where(c =>
                    c.FreelancerId == freelancerId &&
                    c.Status == ContractStatus.Completed);

            // 3. Фильтрация по клиенту
            if (query.ClientId.HasValue)
                qc = qc.Where(c => c.EmployerId == query.ClientId.Value);

            // 4. Фильтрация по диапазону заработка
            if (query.EarnedRange?.Length == 2)
            {
                var min = query.EarnedRange[0];
                var max = query.EarnedRange[1];

                qc = qc.Where(c =>
                    (c.BudgetType == BudgetType.Fixed
                        ? c.Budget.Amount
                        : c.CostPerHour.Amount *
                          c.WorkSessions
                            .Where(ws => ws.EndDate.HasValue)
                            .Sum(ws => (decimal)((ws.EndDate!.Value - ws.StartDate).TotalHours)))
                    >= min
                    &&
                    (c.BudgetType == BudgetType.Fixed
                        ? c.Budget.Amount
                        : c.CostPerHour.Amount *
                          c.WorkSessions
                            .Where(ws => ws.EndDate.HasValue)
                            .Sum(ws => (decimal)((ws.EndDate!.Value - ws.StartDate).TotalHours)))
                    <= max
                );
            }

            // 5. Фильтрация по дате завершения
            if (query.CompletedAfter.HasValue)
                qc = qc.Where(c => c.EndDate >= query.CompletedAfter.Value);

            if (query.CompletedBefore.HasValue)
                qc = qc.Where(c => c.EndDate <= query.CompletedBefore.Value);

            // 6. Сортировка по дате
            qc = qc.OrderByDescending(c => c.EndDate);

            // 7. Пагинация
            if (query.RangeOfContracts?.Length == 2)
            {
                var skip = query.RangeOfContracts[0];
                var take = query.RangeOfContracts[1];
                qc = qc.Skip(skip).Take(take);
            }

            // 8. Проекция в DTO
            var list = await qc.Select(c => new ClientHistoryDto
            {
                ContractId = c.Id,
                JobTitle = c.Job.Title,
                CompletedAt = c.EndDate!.Value,
                ClientName = c.Employer.Fullname,
                ClientAvatar = c.Employer.Avatar,
                AmountEarned = c.BudgetType == BudgetType.Fixed
                                  ? c.Budget.Amount
                                  : c.CostPerHour.Amount *
                                    c.WorkSessions
                                        .Where(ws => ws.EndDate.HasValue)
                                        .Sum(ws => (decimal)((ws.EndDate!.Value - ws.StartDate).TotalHours))
            })
            .ToListAsync();

            return list;
        }
    }
}
