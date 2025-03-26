using Flsurf.Application.Freelance.Commands.Skills;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class SkillService : ISkillService
    {
        private readonly IServiceProvider _serviceProvider;

        public SkillService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateSkillsHandler CreateSkills() =>
            _serviceProvider.GetRequiredService<CreateSkillsHandler>();

        public UpdateSkillsHandler UpdateSkills() =>
            _serviceProvider.GetRequiredService<UpdateSkillsHandler>();

        public DeleteSkillsHandler DeleteSkills() =>
            _serviceProvider.GetRequiredService<DeleteSkillsHandler>();

        public GetSkillsHandler GetSkills() =>
            _serviceProvider.GetRequiredService<GetSkillsHandler>();
    }
}
