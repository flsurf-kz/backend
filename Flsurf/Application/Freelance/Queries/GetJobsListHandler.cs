﻿using Flsurf.Application.Common.cqrs;
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
    public class GetJobsListHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : IQueryHandler<GetJobsListQuery, List<JobEntity>>
    {
        public async Task<List<JobEntity>> Handle(GetJobsListQuery query)
        {
            var jobsQuery = dbContext.Jobs
                .Where(j => j.Status != JobStatus.Draft); // Исключаем черновики

            // 🔥 Фильтр по названию и описанию (поиск)
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string searchLower = query.Search.ToLower();
                jobsQuery = jobsQuery.Where(j =>
                    j.Title.ToLower().Contains(searchLower) ||
                    j.Description.ToLower().Contains(searchLower));
            }

            // 🔥 Фильтр по категории
            if (query.CategoryId.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.CategoryId == query.CategoryId);
            }

            // 🔥 Фильтр по уровню опыта
            if (query.Levels is { Length: > 0 })
            {
                jobsQuery = jobsQuery.Where(j => query.Levels.Contains(j.Level));
            }

            // 🔥 Фильтр по типу оплаты и бюджету
            if (query.IsHourly.HasValue)
            {
                if (query.IsHourly.Value)
                {
                    jobsQuery = jobsQuery.Where(j => j.BudgetType == BudgetType.Hourly);

                    if (query.MinHourlyRate.HasValue)
                        jobsQuery = jobsQuery.Where(j => j.Payout.Amount >= query.MinHourlyRate);

                    if (query.MaxHourlyRate.HasValue)
                        jobsQuery = jobsQuery.Where(j => j.Payout.Amount <= query.MaxHourlyRate);
                }
                else
                {
                    jobsQuery = jobsQuery.Where(j => j.BudgetType == BudgetType.Fixed);

                    if (query.MinBudget.HasValue)
                        jobsQuery = jobsQuery.Where(j => j.Payout.Amount >= query.MinBudget);

                    if (query.MaxBudget.HasValue)
                        jobsQuery = jobsQuery.Where(j => j.Payout.Amount <= query.MaxBudget);
                }
            }

            // 🔥 Фильтр по количеству ставок
            if (query.MinProposals.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.Proposals.Count >= query.MinProposals);
            }
            if (query.MaxProposals.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.Proposals.Count <= query.MaxProposals);
            }

            if (query.MinDurationDays.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.Duration >= query.MinDurationDays);
            }
            if (query.MaxDurationDays.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.Duration <= query.MaxDurationDays);
            }

            if (query.EmployerLocation != null)
            {
                jobsQuery = jobsQuery.Where(j => j.Employer.Location == query.EmployerLocation);
            }

            if (query.Statuses is { Length: > 0 })
            {
                jobsQuery = jobsQuery.Where(j => query.Statuses.Contains(j.Status));
            }

            if (query.ClientId != null)
            {
                jobsQuery = jobsQuery.Where(x => x.EmployerId == query.ClientId); 
            }

            // означает то что это приватные работы и имеют контракт если нет то игнорируем
            if (query.FreelancerId != null)
            {
                var user = await permService.GetCurrentUser();
                if (user.Id != query.FreelancerId && user.Role < Domain.User.Enums.UserRoles.Moderator)
                    throw new AccessDenied("Надо быть модером");
                if (user.Type != Domain.User.Enums.UserTypes.Freelancer)
                    throw new AccessDenied("Не тот тип");

                jobsQuery = jobsQuery.Where(x => 
                    x.Contract != null && 
                    x.Contract.FreelancerId == user.Id && 
                    x.Status != JobStatus.Open); 
            }

            if (query.SortOption != null && query.SortType != null)
                jobsQuery = jobsQuery.OrderBy(x => x.PublicationDate); 

            // 🔥 Сортировка + Пагинация
            var jobs = await jobsQuery
                .OrderByDescending(j => j.PublicationDate)
                .Paginate(query.Start, query.Ends)
                .IncludeStandard() // Загружаем необходимые связи
                .ToListAsync();

            return jobs;
        }
    }

}
