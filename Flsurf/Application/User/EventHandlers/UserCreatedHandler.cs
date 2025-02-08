using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Application.User.Permissions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Domain.User.Events;
using Flsurf.Infrastructure.Adapters.Permissions;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Application.User.EventHandlers
{
    public class UserCreatedHandler : IEventSubscriber<UserCreated>
    {
        private ILogger _logger;
        private IPermissionService _permService; 

        public UserCreatedHandler(ILogger<UserCreatedHandler> logger, IPermissionService permService)
        {
            _logger = logger;
            _permService = permService;
        }

        public async Task HandleEvent(UserCreated eventValue, IApplicationDbContext _context)
        {
            var wallet = WalletEntity.Create(eventValue.User);

            _context.Wallets.Add(wallet);

            _logger.LogInformation($"User created with id: {eventValue.User.Id}");

            await _permService.AddRelationship(
                ZedWallet.WithId(wallet.Id).Owner(ZedPaymentUser.WithId(eventValue.User.Id)));

            return; 
        }
    }
}
