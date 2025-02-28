using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using SpiceDb.Models;

namespace Flsurf.Application.Freelance.Commands.Category.CreateCategory
{
    public class CreateCategoryHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : ICommandHandler<CreateCategoryCommand>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;
        private readonly IPermissionService _permService = permService;

        public async Task<CommandResult> Handle(CreateCategoryCommand command)
        {
            // 🔐 Получаем текущего пользователя
            var user = await _permService.GetCurrentUser();

            // 🔎 Проверяем, может ли пользователь создавать категории
            var hasPermission = await _permService.CheckPermission();

            if (!hasPermission) return CommandResult.Forbidden("Lol no");

            // 🔍 Проверяем, существует ли родительская категория (если указана)
            CategoryEntity? parentCategory = null;
            if (command.ParentCategoryId.HasValue)
            {
                parentCategory = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == command.ParentCategoryId.Value);

                if (parentCategory == null)
                {
                    return CommandResult.NotFound("Parent category not found", command.ParentCategoryId.Value);
                }
            }

            // 🔥 Генерируем `Slug`, если не передан
            var slug = !string.IsNullOrWhiteSpace(command.Slug)
                ? command.Slug.ToLower().Replace(" ", "-")
                : command.Name.ToLower().Replace(" ", "-");

            // 🛠️ Создаём сущность категории
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Slug = slug,
                ParentCategoryId = command.ParentCategoryId,
                ParentCategory = parentCategory,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            // 💾 Сохраняем в БД
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return CommandResult.Success(category.Id);
        }
    }

}
