using Flsurf.Application.Common.cqrs;

namespace Flsurf.Application.Freelance.Commands.Category.UpdateCategory
{
    public class InviteFreelancerToTeamCommand : BaseCommand
    {
        public Guid FreelanceGroupId { get; }
        public Guid UserId { get; }
    }
}
