using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class CreateFreelancerTeamCommand : BaseCommand
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
