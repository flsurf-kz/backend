using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class UpdateSkillsCommand : BaseCommand
    {
        public List<UpdateSkillDto> Skills { get; set; } = [];
    }

    public class UpdateSkillDto
    {
        public Guid SkillId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateSkillsHandler : ICommandHandler<UpdateSkillsCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPermissionService _permService;

        public UpdateSkillsHandler(IApplicationDbContext dbContext, IPermissionService permService)
        {
            _dbContext = dbContext;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(UpdateSkillsCommand command)
        {
            var user = await _permService.GetCurrentUser();

            // Проверяем, может ли пользователь обновлять глобальные скиллы
            if (!await _permService.CheckPermission(ZedFreelancerUser.WithId(user.Id).CanAddGlobalSkills()))
            {
                return CommandResult.Forbidden("У вас нет прав для обновления скиллов.");
            }

            var skillIds = command.Skills.Select(s => s.SkillId).ToList();
            var skills = await _dbContext.Skills.Where(s => skillIds.Contains(s.Id)).ToListAsync();

            if (skills.Count != skillIds.Count)
            {
                return CommandResult.NotFound("Некоторые скиллы не найдены.", skillIds);
            }

            foreach (var updateDto in command.Skills)
            {
                var skill = skills.FirstOrDefault(s => s.Id == updateDto.SkillId);
                if (skill != null)
                {
                    skill.Name = updateDto.Name;
                }
            }

            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(skillIds);
        }
    }
}
