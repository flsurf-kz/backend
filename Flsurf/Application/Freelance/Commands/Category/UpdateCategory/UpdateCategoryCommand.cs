using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommand : BaseCommand
    {
        public Guid CategoryId { get; }
        public string? Name { get; }
    }
}
