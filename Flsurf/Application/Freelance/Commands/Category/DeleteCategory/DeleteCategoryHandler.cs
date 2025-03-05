using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.DeleteCategory
{

    public class DeleteCategoryHandler : ICommandHandler<DeleteCategoryCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public DeleteCategoryHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(DeleteCategoryCommand command)
        {
            var user = await _permService.GetCurrentUser(); // Гарантированно возвращает пользователя

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == command.CategoryId);
            if (category == null)
            {
                return CommandResult.NotFound("Категория не найдена", command.CategoryId);
            }

            // Проверка прав на удаление категории
            bool hasPermission = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanDeleteCategory(ZedCategory.WithId(command.CategoryId))
            );

            if (!hasPermission)
            {
                return CommandResult.Forbidden("Недостаточно прав для удаления категории.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return CommandResult.Success(command.CategoryId);
        }
    }
}
