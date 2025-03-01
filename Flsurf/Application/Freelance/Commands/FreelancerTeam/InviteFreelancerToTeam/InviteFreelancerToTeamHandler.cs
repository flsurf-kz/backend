using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Application.Messaging.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class InviteFreelancerToTeamHandler : ICommandHandler<InviteFreelancerToTeamCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public InviteFreelancerToTeamHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(InviteFreelancerToTeamCommand command)
        {
            var user = await _permService.GetCurrentUser();
            await _permService.CheckPermission(
                ZedFreelanceUser.WithId(user.Id).CanInviteMembers(ZedFreelancerTeam.WithId(command.FreelanceGroupId))); 

            var group = await _context.FreelancerTeams
                .Include(x => x.Participants)
                .Include(x => x.Invitations)
                .FirstOrDefaultAsync(x => x.Id == command.FreelanceGroupId); 

            if (group == null)
            {
                return CommandResult.NotFound("No group", command.FreelanceGroupId); 
            }

            var selectedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId); 
            if (selectedUser == null)
            {
                return CommandResult.NotFound("", command.UserId); 
            }

            group.InviteMember(user);

            await _context.SaveChangesAsync(); 

            return CommandResult.Success(); // Возвращает успешный результат без данных
        }
    }
}
