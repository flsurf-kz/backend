using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Application.Payment.Commands;
using Flsurf.Domain.Payment.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.UseCases
{
    public class HandleDepositGatewayResult : ICommandHandler<GatewayResultCommand>
    {
        private IApplicationDbContext _context;

        public HandleDepositGatewayResult(IApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<CommandResult> Handle(GatewayResultCommand dto)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(dto.InternalTransactionId));

            if (transaction == null)
                return CommandResult.NotFound("Транзакция не найдена", dto.InternalTransactionId);

            if (dto.Success && transaction.Type == TransactionType.Deposit)
            {
                var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == transaction.WalletId);
                if (wallet == null)
                    return CommandResult.NotFound("Кошелек не найден", transaction.WalletId); 

                wallet.AcceptTransaction(transaction);
                transaction.ConfirmFromGateway();
            }
            else
            {
                transaction.Cancel();
            }

            await _context.SaveChangesAsync();

            return CommandResult.Success(transaction.Id);
        }
    }
}
