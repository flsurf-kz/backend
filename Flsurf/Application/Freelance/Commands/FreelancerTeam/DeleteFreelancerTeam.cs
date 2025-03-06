using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerTeam
{
    public class DeleteFreelancerTeamCommand : BaseCommand
    {
        public Guid TeamId { get; set; }
    }


    public class DeleteFreelancerTeamHandler : ICommandHandler<DeleteFreelancerTeamCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public DeleteFreelancerTeamHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(DeleteFreelancerTeamCommand command)
        {
            // 🔐 Получаем текущего пользователя
            var user = await _permService.GetCurrentUser();

            // 🔎 Проверяем, существует ли команда
            var team = await _context.FreelancerTeams.FirstOrDefaultAsync(t => t.Id == command.TeamId);
            if (team == null)
            {
                return CommandResult.NotFound("Freelancer team not found.", command.TeamId);
            }

            // 🔒 Проверяем, является ли `user` владельцем команды
            if (team.OwnerId != user.Id)
            {
                return CommandResult.Forbidden("Only the owner can delete the team.");
            }

            // 🗑 Удаляем команду
            _context.FreelancerTeams.Remove(team);
            await _context.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
