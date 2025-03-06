using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class DeletePortfolioProjectCommand : BaseCommand
    {
        public Guid ProjectId { get; set; }
    }

    public class DeletePortfolioProjectHandler : ICommandHandler<DeletePortfolioProjectCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public DeletePortfolioProjectHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(DeletePortfolioProjectCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var project = await _context.PortfolioProjects
                .FirstOrDefaultAsync(p => p.Id == command.ProjectId);

            if (project == null)
            {
                return CommandResult.NotFound("Project not found", command.ProjectId);
            }

            // Проверяем, является ли пользователь владельцем проекта
            if (project.UserId != user.Id)
            {
                return CommandResult.Forbidden("You can only delete your own portfolio projects");
            }

            _context.PortfolioProjects.Remove(project);
            await _context.SaveChangesAsync();

            return CommandResult.Success();
        }
    }

}
