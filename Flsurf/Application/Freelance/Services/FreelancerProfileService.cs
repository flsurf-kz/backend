using Flsurf.Application.Freelance.Commands.FreelancerProfile;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class FreelancerProfileService : IFreelancerProfileService
    {
        private readonly IServiceProvider _serviceProvider;

        public FreelancerProfileService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateFreelancerProfileHandler CreateFreelancerProfile() =>
            _serviceProvider.GetRequiredService<CreateFreelancerProfileHandler>();

        public HideFreelancerProfileHandler HideFreelancerProfile() =>
            _serviceProvider.GetRequiredService<HideFreelancerProfileHandler>();

        public UpdateFreelancerProfileHandler UpdateFreelancerProfile() =>
            _serviceProvider.GetRequiredService<UpdateFreelancerProfileHandler>();

        public GetFreelancerProfileHandler GetFreelancerProfile() =>
            _serviceProvider.GetRequiredService<GetFreelancerProfileHandler>();

        public GetFreelancerProfileListHandler GetFreelancerProfileList() =>
            _serviceProvider.GetRequiredService<GetFreelancerProfileListHandler>();
    }
}
