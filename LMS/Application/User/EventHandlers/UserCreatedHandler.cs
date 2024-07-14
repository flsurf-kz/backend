using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Enums;
using Flsurf.Domain.User.Events;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Application.User.EventHandlers
{
    public class UserCreatedHandler : IEventSubscriber<UserCreated>
    {
        private ILogger _logger;

        public UserCreatedHandler(ILogger<UserCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleEvent(UserCreated eventValue, IApplicationDbContext _context)
        {
            var wallet = WalletEntity.Create(eventValue.User);

            _context.Wallets.Add(wallet);

            _logger.LogInformation($"User created with id: {eventValue.User.Id}");

            eventValue.User.Permissions.AddPermissionWithCode(wallet, PermissionEnum.all); 

            return Task.CompletedTask;
        }
    }
}
