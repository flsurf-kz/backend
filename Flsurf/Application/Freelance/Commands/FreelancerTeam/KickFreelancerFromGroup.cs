using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerTeam
{
    public class KickFreelancerFromGroupCommand : BaseCommand {  
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
    }

    public class KickFreelancerFromGroup : ICommandHandler<KickFreelancerFromGroupCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public KickFreelancerFromGroup(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(KickFreelancerFromGroupCommand command)
        {
            // 🔐 Получаем текущего пользователя
            var owner = await _permService.GetCurrentUser();

            // 🔎 Проверяем, существует ли команда
            var team = await _context.FreelancerTeams.FirstOrDefaultAsync(t => t.Id == command.TeamId);
            if (team == null)
            {
                return CommandResult.NotFound("Freelancer team not found.", command.TeamId);
            }

            // 🔒 Проверяем, является ли `owner` владельцем команды
            bool isOwner = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(owner.Id).CanKickMembers(ZedFreelancerTeam.WithId(command.TeamId))
            );

            if (!isOwner)
            {
                return CommandResult.Forbidden("Only the owner can kick members.");
            }

            // 🔎 Проверяем, существует ли пользователь
            var freelancer = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
            if (freelancer == null)
            {
                return CommandResult.NotFound("Freelancer not found.", command.UserId);
            }

            // 🔍 Проверяем, является ли он членом команды
            bool isMember = await _permService.CheckPermission(
                ZedGroup.WithId(team.Id).Type, "member", freelancer.Id.ToString()
            );

            if (!isMember)
            {
                return CommandResult.Conflict("User is not a member of the team.");
            }

            // 🔥 Удаляем участника из команды через `FreelancerTeamEntity.KickMember()`
            team.RemoveMember(freelancer);

            // 💾 Сохраняем изменения в БД
            await _context.SaveChangesAsync();

            // ❌ Удаляем `Member` через `DeleteRelationship()`
            await _permService.DeleteRelationship(
                ZedFreelancerTeam.WithId(team.Id).Member(ZedFreelancerUser.WithId(freelancer.Id))
            );

            return CommandResult.Success(freelancer.Id);
        }
    }
}
