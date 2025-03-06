using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerTeam
{
    public class CreateFreelancerTeamCommand : BaseCommand
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CreateFreelancerTeamHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : ICommandHandler<CreateFreelancerTeamCommand>
    {
        private IApplicationDbContext _dbContext = dbContext;
        private IPermissionService _permService = permService;

        public async Task<CommandResult> Handle(CreateFreelancerTeamCommand command)
        {
            // 🔐 Получаем текущего пользователя
            var user = await _permService.GetCurrentUser();

            // 🔎 Проверяем, является ли пользователь фрилансером
            if (user.Type != UserTypes.Freelancer)
            {
                return CommandResult.Forbidden("Only freelancers can create teams.");
            }

            // 🔍 Проверяем, существует ли уже команда с таким именем
            bool nameExists = await _dbContext.FreelancerTeams
                .AnyAsync(t => t.Name.ToLower() == command.Name.ToLower());

            if (nameExists)
            {
                return CommandResult.Conflict("A team with this name already exists.");
            }

            // 🛠️ Создаём сущность команды фрилансеров
            var team = new FreelancerTeamEntity
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                OwnerId = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            // 💾 Сохраняем в БД
            _dbContext.FreelancerTeams.Add(team);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(team.Id);
        }
    }

}
