using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Flsurf.Application.Payment.Commands
{
    public class HandleWithdrawalGatewayResult : ICommandHandler<GatewayResultCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly Regex transactionIdRegex = new(
            "[0-9a-f]{8}-[0-9a-f]{4}-[0-5][0-9a-f]{3}-[089ab][0-9a-f]{3}-[0-9a-f]{12}");

        public HandleWithdrawalGatewayResult(IApplicationDbContext context)
        {
            _context = context;
        }

        private (Guid, string) ExtractTxId(string internalTransactionId, string customId)
        {
            if (Guid.TryParse(internalTransactionId, out var guid))
                return (guid, "");

            Match match = transactionIdRegex.Match(customId);
            if (match.Success)
                return (Guid.Parse(match.Value), "");

            return (Guid.Empty, $"Невозможно извлечь идентификатор транзакции из: {internalTransactionId}, {customId}");
        }

        public async Task<CommandResult> Handle(GatewayResultCommand dto)
        {
            var (txId, err) = ExtractTxId(dto.InternalTransactionId, dto.CustomId);

            if (txId == Guid.Empty)
                return CommandResult.UnprocessableEntity(err);

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(x => x.Id == txId);

            if (transaction == null)
                return CommandResult.NotFound("Транзакция не найдена", txId);

            if (transaction.Type != TransactionType.Withdrawal)
                return CommandResult.UnprocessableEntity("Транзакция не является выводом средств.");

            if (transaction.Status != TransactionStatus.Pending)
                return CommandResult.UnprocessableEntity("Транзакция уже обработана.");

            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == transaction.WalletId);

            if (wallet == null)
                return CommandResult.NotFound("Кошелёк не найден", transaction.WalletId);

            if (dto.Success)
            {
                // Подтверждаем успешный вывод средств
                transaction.ConfirmFromGateway();
            }
            else
            {
                // Произошла ошибка при выводе, возвращаем средства обратно на баланс
                transaction.Cancel();
                wallet.RefundTransactionWithoutReceiver(transaction, new NoFeePolicy());
            }

            await _context.SaveChangesAsync();

            return CommandResult.Success(transaction.Id);
        }
    }

}
