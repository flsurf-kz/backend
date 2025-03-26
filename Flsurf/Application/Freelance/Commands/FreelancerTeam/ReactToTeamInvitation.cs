using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Enums;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.FreelancerTeam
{
    public class ReactToTeamInvitationComman : BaseCommand { 
        public Guid InvitationId { get; set; }
        public bool Accepted { get; set; }
    }

    public class ReactToTeamInvitationHandler : ICommandHandler<ReactToTeamInvitationComman>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public ReactToTeamInvitationHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(ReactToTeamInvitationComman command)
        {
            var invitation = await _context.FreelancerTeamInvitations.FirstOrDefaultAsync(x => x.Id == command.InvitationId);

            if (invitation == null)
            {
                return CommandResult.NotFound("", command.InvitationId); 
            }

            var user = await _permService.GetCurrentUser(); 

            if (invitation.UserId != user.Id)
            {
                return CommandResult.Forbidden("");
            }

            var team = await _context.FreelancerTeams
                .IncludeStandard()
                .FirstOrDefaultAsync(x => x.Id == invitation.FreelancerTeamId);

            if (team == null)
                return CommandResult.Forbidden("");

            team.AddMember(user, invitation);

            await _permService.AddRelationship(
                ZedFreelancerTeam
                    .WithId(team.Id)
                    .Member(ZedFreelancerUser.WithId(user.Id))); 

            await _context.SaveChangesAsync(); 

            return CommandResult.Success(team.Id); 
        }
    }
}
