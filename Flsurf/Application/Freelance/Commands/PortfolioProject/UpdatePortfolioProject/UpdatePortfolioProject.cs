using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class UpdatePortfolioProjectCommand : BaseCommand
    {
        public Guid ProjectId { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? UserRole { get; set; } = string.Empty;
        public List<Guid>? Skills { get; set; } = [];
        public string? Description { get; set; } = string.Empty;
        public List<CreateFileDto>? Images { get; set; } = [];
        public bool? Hidden { get; set; }
    }


    public class UpdatePortfolioProjectHandler : ICommandHandler<UpdatePortfolioProjectCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;
        private readonly UploadFiles _uploadFiles;

        public UpdatePortfolioProjectHandler(
            IApplicationDbContext context,
            IPermissionService permService,
            UploadFiles uploadFiles)
        {
            _context = context;
            _permService = permService;
            _uploadFiles = uploadFiles;
        }

        public async Task<CommandResult> Handle(UpdatePortfolioProjectCommand command)
        {
            var user = await _permService.GetCurrentUser();
            var project = await _context.PortfolioProjects
                .Include(p => p.Images)
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == command.ProjectId);

            if (project == null)
                return CommandResult.NotFound("Portfolio project not found", command.ProjectId);

            if (project.UserId != user.Id)
                return CommandResult.Forbidden("You can only update your own projects.");

            // Обновление полей, если они переданы
            project.Name = command.Name ?? project.Name;
            project.UserRole = command.UserRole ?? project.UserRole;
            project.Description = command.Description ?? project.Description;
            project.Hidden = command.Hidden ?? project.Hidden;

            // Обновление навыков
            if (command.Skills is not null && command.Skills.Any())
            {
                var skills = await _context.Skills.Where(s => command.Skills.Contains(s.Id)).ToListAsync();
                project.Skills = skills;
            }

            // Обновление изображений
            if (command.Images is not null && command.Images.Any())
            {
                var uploadedFiles = await _uploadFiles.Execute(command.Images);
                project.Images = uploadedFiles;
            }

            await _context.SaveChangesAsync();
            return CommandResult.Success(project.Id);
        }
    }
}
