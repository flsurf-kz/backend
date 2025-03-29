using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Payment;
using Flsurf.Infrastructure.Adapters.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.Commands
{
    public class StartPaymentFlowCommand : BaseCommand
    {
        public Money Amount { get; set; } = null!;
        public TransactionFlow Flow { get; set; }
        public TransactionType Type { get; set; }
        public Guid ProviderId { get; set; }
    }

    public class StartPaymentFlowHandler : ICommandHandler<StartPaymentFlowCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPaymentAdapterFactory _paymentAdapterResolver; // через DI
        private readonly IPermissionService _permService; 

        public StartPaymentFlowHandler(IApplicationDbContext context, IPaymentAdapterFactory paymentAdapterResolver, IPermissionService permService)
        {
            _permService = permService; 
            _context = context;
            _paymentAdapterResolver = paymentAdapterResolver;
        }

        public async Task<CommandResult> Handle(StartPaymentFlowCommand command)
        {
            var currentUser = await _permService.GetCurrentUser(); 

            var wallet = await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == currentUser.Id);

            // мало вероятно конечно, но возможен при ошибках в бд
            if (wallet == null)
                throw new NullReferenceException("Критическая ошибка! Нету кошелка у авторизованного пользвателя!");

            var provider = await _context.TransactionProviders
                .Include(p => p.Systems)
                .FirstOrDefaultAsync(p => p.Id == command.ProviderId);

            if (provider == null)
                return CommandResult.NotFound("Платежный провайдер не найден", command.ProviderId);

            var system = provider.GetActiveSystems().FirstOrDefault();
            if (system == null)
                return CommandResult.BadRequest("У провайдера нет активных платёжных систем");

            var adapter = _paymentAdapterResolver.GetPaymentAdapter(system.Name);
            if (adapter == null)
                return CommandResult.BadRequest($"Не найден адаптер для платежной системы: {system.Name}");

            var transactionId = Guid.NewGuid();

            var payload = new PaymentPayload
            {
                Amount = command.Amount.Amount,
                Currency = command.Amount.Currency.ToString(),
                OrderId = transactionId.ToString(),
                Name = $"Оплата через {provider.Name}",
                Custom = $"User:{wallet.UserId}",
                AdditionalInfo = new()
                {
                    { "WalletId", wallet.Id.ToString() },
                    { "TransactionType", command.Type.ToString() }
                }
            };

            var paymentResult = await adapter.InitPayment(payload);

            if (!paymentResult.Success)
                return CommandResult.BadRequest($"Ошибка шлюза: {paymentResult.Message}");

            var props = TransactionPropsEntity.CreateGatewayProps(
                paymentUrl: paymentResult.LinkUrl,
                successUrl: $"https://flsurf.ru/payment/success?tx={transactionId}",
                paymentGateway: system.Name,
                feeContext: new FeeContext()
            );

            var tx = new TransactionEntity(
                walletId: wallet.Id,
                amount: command.Amount,
                feePolicy: new StandardFeePolicy(0, provider.FeePercent),
                type: command.Type,
                flow: command.Flow,
                props: props,
                freezeTimeInDays: null,
                comment: $"Оплата через шлюз: {provider.Name}"
            );

            tx.Id = transactionId;
            tx.Provider = provider;

            await _context.SaveChangesAsync();

            return CommandResult.Success(tx.Id);
        }
    }
}
