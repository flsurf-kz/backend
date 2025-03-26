using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class PortfolioProjectService : IPortfolioProjectService
    {
        private readonly IServiceProvider _serviceProvider;

        public PortfolioProjectService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public AddPortfolioProjectHandler CreatePortfolioProject() =>
            _serviceProvider.GetRequiredService<AddPortfolioProjectHandler>();

        public UpdatePortfolioProjectHandler UpdatePortfolioProject() =>
            _serviceProvider.GetRequiredService<UpdatePortfolioProjectHandler>();

        public DeletePortfolioProjectHandler DeletePortfolioProject() =>
            _serviceProvider.GetRequiredService<DeletePortfolioProjectHandler>();

        public GetPortfolioProjectsHandler GetPortfolioProjects() =>
            _serviceProvider.GetRequiredService<GetPortfolioProjectsHandler>();
    }
}
