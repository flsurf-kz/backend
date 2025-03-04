using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class CreateSkillsCommand : BaseCommand
    {
        public List<string> SkillNames { get; set; } = [];
    }


    public class CreateSkillsHandler : ICommandHandler<CreateSkillsCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public CreateSkillsHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(CreateSkillsCommand command)
        {
            var user = await _permService.GetCurrentUser();

            // Проверка прав на добавление глобальных навыков
            if (!await _permService.CheckPermission("global:skills", "add", $"user:{user.Id}"))
            {
                return CommandResult.Forbidden("You do not have permission to add global skills.");
            }

            if (command.SkillNames == null || command.SkillNames.Count == 0)
            {
                return CommandResult.BadRequest("Skill list cannot be empty.");
            }

            // Убираем дубликаты и проверяем существующие скиллы
            var uniqueSkillNames = command.SkillNames.Distinct().ToList();
            var existingSkills = await _context.Skills
                .Where(s => uniqueSkillNames.Contains(s.Name))
                .Select(s => s.Name)
                .ToListAsync();

            var newSkills = uniqueSkillNames
                .Where(name => !existingSkills.Contains(name))
                .Select(name => SkillEntity.Create(name))
                .ToList();

            if (newSkills.Count == 0)
            {
                return CommandResult.BadRequest("All provided skills already exist.");
            }

            _context.Skills.AddRange(newSkills);
            await _context.SaveChangesAsync();

            return CommandResult.Success();
        }
    }

}
