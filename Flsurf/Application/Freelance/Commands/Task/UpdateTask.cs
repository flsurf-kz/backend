using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Flsurf.Application.Freelance.Commands.Task
{
    public class UpdateTaskCommand : BaseCommand
    {
        public Guid TaskId { get; set; }
        public string? Title { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public int? Priority { get; set; }
    }

    public class UpdateTaskHandler : ICommandHandler<UpdateTaskCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPermissionService _permService;

        public UpdateTaskHandler(IApplicationDbContext db, IPermissionService permService)
        {
            _db = db;
            _permService = permService; 
        }

        public async Task<CommandResult> Handle(UpdateTaskCommand command)
        {
            var task = await _db.Tasks
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(x => x.Id == command.TaskId);

            var currentUser = await _permService.GetCurrentUser(); 

            if (task == null)
                return CommandResult.NotFound("Задача не найдена", command.TaskId);

            if (currentUser.Type != Domain.User.Enums.UserTypes.Client)
                return CommandResult.Forbidden("Нет прав на обновление");

            if (task.Status == "Approved")
                return CommandResult.BadRequest("Нельзя изменить одобренную задачу");

            task.Update(
                command.Title ?? task.TaskTitle, 
                command.Description ?? task.TaskDescription, 
                command.Priority ?? task.Priority); 

            return CommandResult.Success(task.Id);
        }
    }
}
