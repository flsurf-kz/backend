using Flsurf.Application.Freelance.Commands.FreelancerTeam;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class FreelancerTeamService : IFreelancerTeamService
    {
        private readonly IServiceProvider _provider;

        public FreelancerTeamService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public CreateFreelancerTeamHandler CreateFreelancerTeam()
        {
            return _provider.GetRequiredService<CreateFreelancerTeamHandler>();
        }

        public UpdateFreelancerTeamHandler UpdateFreelancerTeam()
        {
            return _provider.GetRequiredService<UpdateFreelancerTeamHandler>();
        }

        public DeleteFreelancerTeamHandler DeleteFreelancerTeam()
        {
            return _provider.GetRequiredService<DeleteFreelancerTeamHandler>();
        }

        public GetFreelancerTeamsHandler GetFreelancerTeams()
        {
            return _provider.GetRequiredService<GetFreelancerTeamsHandler>();
        }

        public KickFreelancerFromGroup KickFreelancerFromGroup()
        {
            return _provider.GetRequiredService<KickFreelancerFromGroup>();
        }

        public ReactToTeamInvitationHandler ReactToTeamInvitation()
        {
            return _provider.GetRequiredService<ReactToTeamInvitationHandler>();
        }

        public InviteFreelancerToTeamHandler InviteFreelancerToTeam()
        {
            return _provider.GetRequiredService<InviteFreelancerToTeamHandler>();
        }
    }
}
