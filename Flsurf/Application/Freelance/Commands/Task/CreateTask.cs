using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Flsurf.Application.Freelance.Commands.Task
{
    public class CreateTaskCommand : BaseCommand
    {
        public Guid ContractId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Priority { get; set; }
    }

    public class CreateTaskHandler : ICommandHandler<CreateTaskCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPermissionService _permService;

        public CreateTaskHandler(IApplicationDbContext db, IPermissionService permService)
        {
            _db = db;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(CreateTaskCommand command)
        {
            var contract = await _db.Contracts.FirstOrDefaultAsync(x => x.Id == command.ContractId);

            var currentUser = await _permService.GetCurrentUser();

            if (contract == null)
                return CommandResult.NotFound("Контракт не найден", command.ContractId);

            if (currentUser.Type != Domain.User.Enums.UserTypes.Client)
                return CommandResult.Forbidden("Нет прав на создание задачи");

            var task = TaskEntity.Create(command.ContractId, command.Title, command.Description, command.Priority); 

            _db.Tasks.Add(task);
            return CommandResult.Success(task.Id);
        }
    }
}
