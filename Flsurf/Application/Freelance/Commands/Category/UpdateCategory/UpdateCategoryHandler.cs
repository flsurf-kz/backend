using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class UpdateCategoryHandler : ICommandHandler<UpdateCategoryCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPermissionService _permService;

        public UpdateCategoryHandler(IApplicationDbContext context, IPermissionService permService)
        {
            _context = context;
            _permService = permService;
        }

        public async Task<CommandResult> Handle(UpdateCategoryCommand command)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == command.CategoryId);

            if (category == null)
            {
                return CommandResult.NotFound("no category", command.CategoryId); 
            }

            var user = await _permService.GetCurrentUser();

            bool ok = await _permService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanUpdateCategory(ZedCategory.WithId(command.CategoryId))); 

            if (!ok)
            {
                return CommandResult.Forbidden("No admin power"); 
            }

            if (!string.IsNullOrWhiteSpace(command.Name))
            {
                category.ChangeName(command.Name); 
            }

            await _context.SaveChangesAsync(); 

            return CommandResult.Success(category.Id); 
        }
    }
}
