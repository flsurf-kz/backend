using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Flsurf.Application.Payment.Commands
{
    public class StartPaymentFlowCommand : BaseCommand
    {
        public Money Amount { get; set; } = null!;
        public TransactionFlow Flow { get; set; }
        public TransactionType Type { get; set; }
        public Guid ProviderId { get; set; }

        // ①  Если пополняем через сохранённую карту
        public Guid? PaymentMethodId { get; init; }

        // ②  Если пользователь ввёл карту и получил token «на лету»
        public string? OneTimeToken { get; init; }
    }

    public class StartPaymentFlowHandler
    : ICommandHandler<StartPaymentFlowCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPaymentAdapterFactory _adapters;
        private readonly IPermissionService _perm;
        private readonly IUrlBuilder _url;   // сервис, собирающий фронт‑URL

        public StartPaymentFlowHandler(IApplicationDbContext db,
                                       IPaymentAdapterFactory adapters,
                                       IPermissionService perm,
                                       IUrlBuilder url)
        {
            _db = db;
            _adapters = adapters;
            _perm = perm;
            _url = url;
        }

        public async Task<CommandResult> Handle(StartPaymentFlowCommand c)
        {
            var user = await _perm.GetCurrentUser();
            var wallet = await _db.Wallets
                .Include(w => w.Transactions)
                .FirstAsync(w => w.UserId == user.Id);

            var provider = await _db.TransactionProviders
                .Include(p => p.Systems)
                .FirstAsync(p => p.Id == c.ProviderId);
            var system = provider.GetActiveSystems().FirstOrDefault()
                           ?? throw new DomainException("ПС неактивна");

            var adapter = _adapters.GetPaymentAdapter(system.Name);

            // token?
            string? token = null;
            if (c.PaymentMethodId is { } pmId)
                token = (await _db.PaymentMethods
                           .FirstAsync(x => x.Id == pmId && x.UserId == user.Id)).Token;
            else if (!string.IsNullOrEmpty(c.OneTimeToken))
                token = c.OneTimeToken;

            var txId = Guid.NewGuid();
            var tx = TransactionEntity.CreateWithProvider(
                wallet.Id, c.Amount, c.Flow, c.Type,
                props: TransactionPropsEntity.CreateGatewayProps(
                    paymentUrl: "", successUrl: _url.Success(txId),
                    paymentGateway: system.Name, feeContext: new FeeContext(), ""),
                provider: provider,
                feePolicy: new StandardFeePolicy(0, provider.FeePercent));
            tx.Id = txId;

            var init = await adapter.InitPayment(
                new PaymentInitRequest(token ?? string.Empty,
                                       c.Amount.Amount,
                                       c.Amount.Currency.ToString(),
                                       txId.ToString(),
                                       $"Deposit {user.Id}",
                                       _url.Success(txId)));

            if (!init.Success)
                return CommandResult.BadRequest("Gateway error");

            tx.Props = TransactionPropsEntity.CreateGatewayProps(
                init.RedirectUrl ?? string.Empty,
                _url.Success(txId),
                system.Name,
                new FeeContext(),
                init.ProviderPaymentId);

            _db.Transactions.Add(tx);
            await _db.SaveChangesAsync();

            return CommandResult.Success(tx.Id);
        }
    }
}
