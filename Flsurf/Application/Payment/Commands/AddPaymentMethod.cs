using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Commands
{
    public class AddPaymentMethodCommand : BaseCommand
    {
        public Guid ProviderId { get; set; } 
        public string PaymentMethodToken { get; set; } = null!;
        public bool MakeDefault { get; set; } = false; 
    }

    public class AddPaymentMethodHandler(IApplicationDbContext dbContext, IPermissionService permService, IPaymentAdapterFactory factory)
        : ICommandHandler<AddPaymentMethodCommand>
    {
        public async Task<CommandResult> Handle(AddPaymentMethodCommand command)
        {
            var currentUser = await permService.GetCurrentUser(); 

            var provider = await dbContext.TransactionProviders
                                    .FirstAsync(p => p.Id == command.ProviderId);

            var adapter = factory.GetPaymentAdapter(provider.Name); 

            // ①  берём метаданные
            var meta = await adapter.FetchCardMetaAsync(command.PaymentMethodToken)
                       ?? throw new DomainException("Adapter did not return metadata");
            
            var existingPaymentMethod = await dbContext.PaymentMethods.FirstOrDefaultAsync(
                x => x.Brand == meta.Brand
                     && x.ExpMonth == meta.ExpMonth 
                     && x.ExpYear == meta.ExpYear 
                     && x.Last4 == meta.Last4
                     && x.UserId == currentUser.Id);
            if (existingPaymentMethod != null)
            {
                return CommandResult.Success(existingPaymentMethod.Id);
            }
            
            // ②  сбрасываем старый default если нужно
            if (command.MakeDefault)
            { 
                await dbContext.PaymentMethods
                    .Where(x => x.UserId == currentUser.Id && x.IsDefault)
                    .ExecuteUpdateAsync(s => s.SetProperty(pm => pm.IsDefault, false));
            }

            var pm = PaymentMethodEntity.Create(
                currentUser.Id, provider,
                token: command.PaymentMethodToken,
                last4: meta.Last4,
                brand: meta.Brand,
                expMonth: meta.ExpMonth,
                expYear: meta.ExpYear,
                isDefault: command.MakeDefault);

            dbContext.PaymentMethods.Add(pm);
            await dbContext.SaveChangesAsync();

            return CommandResult.Success(pm.Id);    // map → masked dto
        }
    }

}
