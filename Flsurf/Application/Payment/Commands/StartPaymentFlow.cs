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
    public class StartPaymentFlowResult
    {
        public Guid InternalTransactionId { get; set; }
        public string ProviderPaymentId { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUrl { get; set; }
    }

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

    public sealed class StartPaymentFlowHandler(
            IApplicationDbContext      db,
            IPaymentAdapterFactory     adapters,
            IPermissionService         perm,
            IUrlBuilder                urls)
        : ICommandHandler<StartPaymentFlowCommand>
    {
        public async Task<CommandResult> Handle(StartPaymentFlowCommand c)
        {
            /* 0. Автор и кошелёк */
            var user   = await perm.GetCurrentUser();
            var wallet = await db.Wallets
                                 .Include(w => w.Transactions)
                                 .FirstAsync(w => w.UserId == user.Id);

            /* 1. Платёжный провайдер / система / адаптер */
            var provider = await db.TransactionProviders
                .Include(x => x.Systems)
                .FirstAsync(p => p.Id == c.ProviderId);
            
            Console.WriteLine($"Активные системы {provider.Systems.Count}");
            var system = provider.GetActiveSystems().FirstOrDefault()
                         ?? throw new DomainException("Нет активной платёжной системы");
            var adapter = adapters.GetPaymentAdapter(provider.Name);

            /* 2. token карты */
            string? cardToken = null;
            if (c.PaymentMethodId is Guid pmId)
            {
                var pm = await db.PaymentMethods
                                 .FirstOrDefaultAsync(x => x.Id == pmId && x.UserId == user.Id);
                if (pm is null)               return CommandResult.NotFound("Карта не найдена", pmId);
                if (pm.ProviderId != c.ProviderId)
                    return CommandResult.BadRequest("Карта относится к другому провайдеру");
                cardToken = pm.Token;
            }
            else if (!string.IsNullOrWhiteSpace(c.OneTimeToken))
            {
                cardToken = c.OneTimeToken;
            }

            /* 3. черновик транзакции */
            var tx = TransactionEntity.CreateWithProvider(
                         walletId : wallet.Id,
                         amount   : c.Amount,
                         flow     : c.Flow,            // Incoming / Outgoing
                         type     : c.Type,            // Deposit  / Withdrawal
                         props    : TransactionPropsEntity.CreateGatewayProps(
                                       paymentUrl  : "",
                                       successUrl  : "",
                                       paymentGateway     : system.Name,
                                       feeContext         : new FeeContext(),
                                       providerPaymentId  : ""),
                         provider : provider,
                         feePolicy: new StandardFeePolicy(0, provider.FeePercent));

            db.Transactions.Add(tx);
            await db.SaveChangesAsync();               // нужен Id

            /* 4. Ветка DEPOSIT ---------------------------------------------------*/
            if (c.Flow == TransactionFlow.Incoming &&
                c.Type == TransactionType.Deposit)
            {
                var init = await adapter.InitPayment(
                    new PaymentInitRequest(
                            cardToken        ?? string.Empty,
                            c.Amount.Amount,
                            c.Amount.Currency.ToString(),
                            tx.Id.ToString(),
                            $"Deposit {user.Id}",
                            urls.Success(tx.Id)));

                if (!init.Success) 
                    return CommandResult.BadRequest("Не смогл создать оплату" + init.ErrorMessage);

                tx.Props.PaymentUrl = init.RedirectUrl ?? "";
                tx.Props.ProviderPaymentId = init.ProviderPaymentId ?? "";
                tx.Props.ClientSecret = init.ClientSecret ?? "";
                
                
                await db.SaveChangesAsync();
                return CommandResult.Success(new StartPaymentFlowResult() { 
                    InternalTransactionId = tx.Id,
                    ProviderPaymentId = init.ProviderPaymentId ?? "",
                    ClientSecret = init.ClientSecret,
                    RedirectUrl = init.RedirectUrl, 
                });
            }
            /* 5. Ветка WITHDRAW --------------------------------------------------*/
            if (c.Flow == TransactionFlow.Outgoing &&
            c.Type == TransactionType.Withdrawal)
            {
                /* 0) валидации */
                if (c.PaymentMethodId is null)
                    return CommandResult.BadRequest("Не выбрана карта для вывода.");

                if (wallet.AvailableBalance < c.Amount)
                    return CommandResult.Conflict("Недостаточно средств на балансе.");

                /* 1) достаём карту */
                var method = await db.PaymentMethods
                    .Include(m => m.Provider)
                    .FirstOrDefaultAsync(m =>
                          m.Id == c.PaymentMethodId &&
                          m.UserId == user.Id &&
                          m.IsActive);

                if (method is null)
                    return CommandResult.NotFound("Карта не найдена", c.PaymentMethodId ?? Guid.Empty);

                /* 3) создаём транзакцию */
                var txId = Guid.NewGuid();
                tx   = TransactionEntity.CreateWithProvider(
                               wallet.Id, c.Amount,
                               TransactionFlow.Outgoing,
                               TransactionType.Withdrawal,
                               TransactionPropsEntity.CreateGatewayProps(
                                   "",                         // нет redirect’а
                                   "",                         // success url придёт из веб-хука
                                   method.Provider.Name,
                                   new FeeContext(),
                                   ""),                        // providerPayoutId заполним позже
                               method.Provider,
                               new StandardFeePolicy(0, method.Provider.FeePercent));

                tx.Id = txId;
                wallet.BalanceOperation(c.Amount, BalanceOperationType.Freeze);               // блокируем до подтверждения

                /* 4) вызы-ваем payout */
                adapter = adapters.GetPaymentAdapter(method.Provider.Name);
                var payout  = await adapter.InitPayoutAsync(
                                  new PayoutInitRequest(
                                      "",   // TODO: It does not work because we dont have connected account of that type 
                                      method.Token,                // card_… – external account
                                      c.Amount.Amount,
                                      c.Amount.Currency.ToString(),
                                      txId.ToString()));

                if (!payout.Success)
                    return CommandResult.BadRequest(
                               "Ошибка payout: " + payout.FailureMessage);

                tx.Props.ProviderPaymentId = payout.ProviderPayoutId;

                db.Transactions.Add(tx);
                await db.SaveChangesAsync();

                /* фронту достаточно ID – статус придёт веб-хуком */
                return CommandResult.Success(tx.Id);
            }
            return CommandResult.BadRequest("Неподдерживаемая комбинация Flow / Type");
        }
    }
}
