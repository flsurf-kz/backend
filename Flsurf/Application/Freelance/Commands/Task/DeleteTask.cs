﻿using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Flsurf.Application.Freelance.Commands.Task
{
    public class DeleteTaskCommand : BaseCommand
    {
        public Guid TaskId { get; set; }
    }

    public class DeleteTaskHandler : ICommandHandler<DeleteTaskCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPermissionService _permService;

        public DeleteTaskHandler(IApplicationDbContext db, IPermissionService permService)
        {
            _db = db;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(DeleteTaskCommand command)
        {
            var task = await _db.Tasks
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(x => x.Id == command.TaskId);

            var currentUser = await _permService.GetCurrentUser(); 

            if (task == null)
                return CommandResult.NotFound("Задача не найдена", command.TaskId);

            if (currentUser.Type != Domain.User.Enums.UserTypes.Client)
                return CommandResult.Forbidden("Нет прав на удаление");

            _db.Tasks.Remove(task);

            return CommandResult.Success(task.Id);
        }
    }

}
