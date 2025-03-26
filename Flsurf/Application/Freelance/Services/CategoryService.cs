using Flsurf.Application.Freelance.Commands.Category;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IServiceProvider _serviceProvider;

        public CategoryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateCategoryHandler CreateCategory() =>
            _serviceProvider.GetRequiredService<CreateCategoryHandler>();

        public UpdateCategoryHandler UpdateCategory() =>
            _serviceProvider.GetRequiredService<UpdateCategoryHandler>();

        public DeleteCategoryHandler DeleteCategory() =>
            _serviceProvider.GetRequiredService<DeleteCategoryHandler>();

        public GetCategoriesHandler GetCategories() =>
            _serviceProvider.GetRequiredService<GetCategoriesHandler>();
    }
}
