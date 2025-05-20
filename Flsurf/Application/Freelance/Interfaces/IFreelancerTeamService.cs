using Flsurf.Application.Freelance.Commands.FreelancerTeam;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IFreelancerTeamService
    {
        // Команды
        CreateFreelancerTeamHandler CreateFreelancerTeam();
        UpdateFreelancerTeamHandler UpdateFreelancerTeam();
        DeleteFreelancerTeamHandler DeleteFreelancerTeam();
        KickFreelancerFromGroup KickFreelancerFromGroup();
        ReactToTeamInvitationHandler ReactToTeamInvitation();
        InviteFreelancerToTeamHandler InviteFreelancerToTeam(); 

        // Запрос
        GetFreelancerTeamsHandler GetFreelancerTeams(); // если он существует как Handler
    }
}
