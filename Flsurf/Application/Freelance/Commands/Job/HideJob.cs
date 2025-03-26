using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Job
{
    public class HideJobCommand : BaseCommand
    {
        public Guid JobId { get; set; }
    }

    public class HideJobHandler : ICommandHandler<HideJobCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public HideJobHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(HideJobCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId);
            if (job == null)
                return CommandResult.NotFound("Вакансия не найдена.", command.JobId);

            // Проверка: скрывать вакансию может только её владелец (клиент)
            if (job.EmployerId != currentUser.Id)
                return CommandResult.Forbidden("Нет прав для скрытия этой вакансии.");

            job.IsHidden = true; // Предполагается, что в JobEntity есть свойство IsHidden
            await _context.SaveChangesAsync();
            return CommandResult.Success(job.Id);
        }
    }
}
