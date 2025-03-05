using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class DeleteSkillsCommand : BaseCommand
    {
        public List<Guid>? SkillIds { get; set; } = [];
        public List<string>? SkillNames { get; set; } = [];
    }

    public class DeleteSkillsHandler : ICommandHandler<DeleteSkillsCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public DeleteSkillsHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(DeleteSkillsCommand command)
        {
            var user = await _permService.GetCurrentUser();

            if (!await _permService.CheckPermission(ZedFreelancerUser.WithId(user.Id).CanAddGlobalSkills()))
            {
                return CommandResult.Forbidden("Недостаточно прав");
            }

            var skillsToDelete = new List<SkillEntity>();

            if (command.SkillIds is { Count: > 0 })
            {
                skillsToDelete.AddRange(await _dbContext.Skills
                    .Where(skill => command.SkillIds.Contains(skill.Id))
                    .ToListAsync());
            }

            if (command.SkillNames is { Count: > 0 })
            {
                skillsToDelete.AddRange(await _dbContext.Skills
                    .Where(skill => command.SkillNames.Contains(skill.Name))
                    .ToListAsync());
            }

            if (!skillsToDelete.Any())
            {
                return CommandResult.NotFound("Навыки не найдены", Guid.Empty);
            }

            _dbContext.Skills.RemoveRange(skillsToDelete);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success();
        }
    }

}
