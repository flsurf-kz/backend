// Application/Payment/Commands/HandleGatewayResultHandler.cs
using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Flsurf.Application.Payment.Commands
{
    public class HandleGatewayResultHandler
        : ICommandHandler<GatewayResultCommand>
    {
        private readonly IApplicationDbContext _db;
        private readonly ILogger<HandleGatewayResultHandler> _log;
        private static readonly Regex GuidRx =
            new("[0-9a-f]{8}-[0-9a-f]{4}-[0-5][0-9a-f]{3}-[089ab][0-9a-f]{3}-[0-9a-f]{12}",
                  RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public HandleGatewayResultHandler(
            IApplicationDbContext db,
            ILogger<HandleGatewayResultHandler> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<CommandResult> Handle(GatewayResultCommand cmd)
        {
            // 1) извлечь Guid внутреннего tx
            if (!TryExtractTxId(cmd.InternalTransactionId, cmd.CustomId, out var txId, out var err))
                return CommandResult.UnprocessableEntity(err);

            // 2) загрузить транзакцию + Props
            var tx = await _db.Transactions
                              .Include(t => t.Props)
                              .FirstOrDefaultAsync(t => t.Id == txId);
            if (tx == null)
                return CommandResult.NotFound("Транзакция не найдена", txId);

            // 3) только Pending обрабатываем
            if (tx.Status != TransactionStatus.Pending)
            {
                _log.LogInformation("Tx {TxId} already {Status}", txId, tx.Status);
                return CommandResult.Success("Транзакция уже обработана", txId);
            }

            // 4) сохранить ID провайдера
            if (tx.Props is null)
                return CommandResult.BadRequest("Props не инициализированы");
            tx.Props.SetProviderPaymentId(cmd.GatewayTransactionId);


            // 5) в случае ошибки – записать комментарий и откатить
            if (!cmd.Success)
            {
                tx.Cancel();
                tx.SetComment(cmd.FailureReason);

                // если это вывод, вернуть деньги
                if (tx.Flow == TransactionFlow.Outgoing)
                {
                    var wallet = await _db.Wallets
                        .FirstOrDefaultAsync(w => w.Id == tx.WalletId)
                        ?? throw new InvalidOperationException("Wallet missing");
                    wallet.RefundTransactionWithoutReceiver(tx, new NoFeePolicy());
                }

                await _db.SaveChangesAsync();
                return CommandResult.Success("Транзакция отменена", txId);
            }

            // 6) успех – применить транзакцию к балансу
            var walletEntity = await _db.Wallets
                .FirstOrDefaultAsync(w => w.Id == tx.WalletId);
            if (walletEntity == null)
                return CommandResult.NotFound("Кошелёк не найден", tx.WalletId);

            tx.ConfirmFromGateway();
            walletEntity.AcceptTransaction(tx);
            await _db.SaveChangesAsync();
            return CommandResult.Success(txId);
        }

        private static bool TryExtractTxId(
            string internalId,
            string customId,
            out Guid id,
            out string error)
        {
            error = string.Empty;
            if (Guid.TryParse(internalId, out id))
                return true;

            var m = GuidRx.Match(customId);
            if (m.Success && Guid.TryParse(m.Value, out id))
                return true;

            id = Guid.Empty;
            error = $"Не удалось извлечь Guid транзакции из '{internalId}' / '{customId}'";
            return false;
        }
    }
}
