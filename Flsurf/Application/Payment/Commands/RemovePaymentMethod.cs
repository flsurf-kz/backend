using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Commands
{
    public class RemovePaymentMethodCommand : BaseCommand 
    {
        public Guid MethId { get; set; }
    }


    public class RemovePaymentMethodHandler(IApplicationDbContext dbContext, IPermissionService permService)
        : ICommandHandler<RemovePaymentMethodCommand>
    {
        public async Task<CommandResult> Handle(RemovePaymentMethodCommand command)
        {
            var user = await permService.GetCurrentUser();

            var pm = await dbContext.PaymentMethods
                .FirstOrDefaultAsync(x => x.Id == command.MethId && x.UserId == user.Id);
            if (pm == null)
                return CommandResult.NotFound("Не найдено", command.MethId); 
            if (pm.IsDefault)
                return CommandResult.Conflict("Нельзя удалить "); 

            pm.Deactivate();
            await dbContext.SaveChangesAsync();
            return CommandResult.Success(pm.Id);
        }
    }
}
