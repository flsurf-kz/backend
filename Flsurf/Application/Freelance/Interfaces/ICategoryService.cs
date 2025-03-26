using Flsurf.Application.Freelance.Commands.Category;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface ICategoryService
    {
        CreateCategoryHandler CreateCategory();
        UpdateCategoryHandler UpdateCategory();
        DeleteCategoryHandler DeleteCategory();
        GetCategoriesHandler GetCategories();
    }
}
