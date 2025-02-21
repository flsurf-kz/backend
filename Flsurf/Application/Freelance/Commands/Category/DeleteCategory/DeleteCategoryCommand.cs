using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Commands.Category.DeleteCategory
{
    public class DeleteCategoryCommand : BaseCommand
    {
        public Guid CategoryId { get; }
    }
}
