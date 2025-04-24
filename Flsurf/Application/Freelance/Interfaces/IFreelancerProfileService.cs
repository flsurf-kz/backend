using Flsurf.Application.Freelance.Commands.FreelancerProfile;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IFreelancerProfileService
    {
        // Команды
        CreateFreelancerProfileHandler CreateFreelancerProfile();
        HideFreelancerProfileHandler HideFreelancerProfile();
        UpdateFreelancerProfileHandler UpdateFreelancerProfile();

        // Запросы
        GetFreelancerProfileHandler GetFreelancerProfile();
        GetFreelancerProfileListHandler GetFreelancerProfileList();
        GetFreelancerStatsHandler GetFreelancerStats();
    }
}
