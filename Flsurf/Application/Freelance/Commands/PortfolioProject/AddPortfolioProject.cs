using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Files.Dto;
using Flsurf.Application.Files.UseCases;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class AddPortfolioProjectCommand : BaseCommand
    {
        public string Name { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty; // Роль в проекте
        public List<Guid> Skills { get; set; } = [];
        public string Description { get; set; } = string.Empty;
        public List<CreateFileDto> Files { get; set; } = []; // Загружаемые файлы (изображения)
    }

    public class AddPortfolioProjectHandler : ICommandHandler<AddPortfolioProjectCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;
        private readonly UploadFiles _uploadFiles;

        public AddPortfolioProjectHandler(IApplicationDbContext dbContext, IPermissionService permService, UploadFiles uploadFiles)
        {
            _dbContext = dbContext;
            _permService = permService;
            _uploadFiles = uploadFiles;
        }

        public async Task<CommandResult> Handle(AddPortfolioProjectCommand command)
        {
            var currentUser = await _permService.GetCurrentUser();

            var profile = await _dbContext.FreelancerProfiles
                .Include(x => x.PortfolioProjects)
                .FirstOrDefaultAsync(x => x.UserId == currentUser.Id);

            if (profile == null)
            {
                return CommandResult.NotFound("Freelancer profile not found.", currentUser.Id);
            }

            var skills = await _dbContext.Skills
                .Where(s => command.Skills.Contains(s.Id))
                .ToListAsync();

            // Используем UploadFiles для загрузки файлов
            var uploadedFiles = await _uploadFiles.Execute(command.Files);

            var project = PortfolioProjectEntity.Create(
                command.Name,
                command.UserRole,
                skills,
                command.Description,
                uploadedFiles,
                currentUser.Id
            );

            profile.PortfolioProjects.Add(project);
            _dbContext.PortfolioProjects.Add(project);

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(project.Id);
        }
    }
}
