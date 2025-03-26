using Flsurf.Application.Freelance.Commands.Category.UpdateCategory;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IPortfolioProjectService
    {
        AddPortfolioProjectHandler CreatePortfolioProject();
        UpdatePortfolioProjectHandler UpdatePortfolioProject();
        DeletePortfolioProjectHandler DeletePortfolioProject();
        GetPortfolioProjectsHandler GetPortfolioProjects();
    }
}
