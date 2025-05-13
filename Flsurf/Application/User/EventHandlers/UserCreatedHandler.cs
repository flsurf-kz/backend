using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Permissions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.User.Events;
using Flsurf.Infrastructure.Adapters.Permissions;

namespace Flsurf.Application.User.DomainHandlers;

public class UserCreatedDomainHandler(
        ILogger<UserCreatedDomainHandler> logger,
        IPermissionService permService)
    : IDomainEventSubscriber<UserCreated>
{
    public async Task HandleEvent(UserCreated evt, IApplicationDbContext ctx)
    {
        // пользователь уже в ChangeTracker, БД не трогаем
        var user = ctx.ChangeTracker.Entries<UserEntity>()
                           .First(e => e.Entity.Id == evt.UserId).Entity;

        var wallet = WalletEntity.Create(user);
        ctx.Wallets.Add(wallet);

        logger.LogInformation("DOMAIN‑handler → wallet created for user {Id}", user.Id);

        // разрешения можно оставить здесь, если сервис использует тот же контекст
        await permService.AddRelationship(
            ZedWallet.WithId(wallet.Id).Owner(ZedPaymentUser.WithId(user.Id)));
    }
}
