using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Freelance.Permissions;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Freelance.Commands.ClientProfile
{
    public class SuspendClientProfileCommand : BaseCommand
    {
        public Guid ClientId { get; set; }
        public string Reason { get; set; } = "Profile suspended for investigation.";
    }


    public class SuspendClientProfileHandler(
        IApplicationDbContext dbContext,
        IPermissionService permService)
        : ICommandHandler<SuspendClientProfileCommand>
    {
        public async Task<CommandResult> Handle(SuspendClientProfileCommand command)
        {
            var user = await permService.GetCurrentUser();

            // Проверка прав на приостановку профиля
            bool hasPermission = await permService.CheckPermission(
                ZedFreelancerUser.WithId(user.Id).CanSuspendClientProfile(ZedClientProfile.WithId(command.ClientId))
            );

            if (!hasPermission)
            {
                return CommandResult.Forbidden("You do not have permission to suspend this client profile.");
            }

            var profile = await dbContext.ClientProfiles
                .FirstOrDefaultAsync(x => x.UserId == command.ClientId);

            if (profile == null)
            {
                return CommandResult.NotFound("Client profile not found.", user.Id);
            }

            profile.Suspended = true;

            await dbContext.SaveChangesAsync();

            return CommandResult.Success(profile.Id);
        }
    }

}
