using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.User.Commands
{
    public class HideNotificationsCommand : BaseCommand {
        public Guid[] NotificationIds { get; set; } = []; 
    }

    public class HideNotifications(IApplicationDbContext dbContext, IPermissionService permService) : ICommandHandler<HideNotificationsCommand>
    {
        public async Task<CommandResult> Handle(HideNotificationsCommand command)
        {
            var user = await permService.GetCurrentUser(); 

            await dbContext.Notifications
                .Include(x => x.Icon)
                .Where(x => command.NotificationIds.Contains(x.Id))
                .Where(x => x.ToUserId == user.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(n => n.Hidden, n => false));

            return CommandResult.Success(); 
        }
    }
}
