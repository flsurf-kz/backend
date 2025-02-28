using Flsurf.Application.Common.cqrs;
using System.Windows.Input;

namespace Flsurf.Application.Freelance.Commands.Category.CreateCategory
{
    public class CreateCategoryCommand : BaseCommand
    {
        public required string Name { get; set; }
        public string? Slug { get; set; }
        public string? Tags { get; set; }
        public Guid? ParentCategoryId { get; set; }
    }
}
