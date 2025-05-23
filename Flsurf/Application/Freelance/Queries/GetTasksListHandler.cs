﻿using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetTasksListHandler(IPermissionService permService, IApplicationDbContext dbContext)
        : IQueryHandler<GetTasksListQuery, List<TaskEntity>>
    {
        private readonly IPermissionService _permService = permService;
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<List<TaskEntity>> Handle(GetTasksListQuery query)
        {
            // 🔐 Получаем текущего пользователя
            var user = await _permService.GetCurrentUser();
            if (user == null) throw new UnauthorizedAccessException("User not authenticated");

            // 🔎 Проверяем, может ли пользователь читать задачи в этом контракте
            var hasPermission = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanReadContract(ZedContract.WithId(query.ContractId)));

            if (!hasPermission) throw new UnauthorizedAccessException("User has no access to contract tasks");

            // 🔥 Получаем задачи контракта
            var tasksQuery = _dbContext.Tasks
                .Where(t => t.ContractId == query.ContractId) // Фильтр по контракту
                .OrderBy(t => t.Priority) 
                .ThenBy(t => t.CreatedAt);

            var tasks = await tasksQuery
                .Paginate(query.Start, query.Ends)
                .ToListAsync();

            return tasks;
        }
    }

}
