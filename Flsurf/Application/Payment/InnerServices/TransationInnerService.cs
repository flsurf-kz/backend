using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Exceptions;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Application.Payment.InnerServices
{


    public class TransactionInnerService(IApplicationDbContext dbContext)
    {
        private IApplicationDbContext _dbContext = dbContext; 

        /// <summary>
        /// Универсальная команда для внутреннего перевода между кошельками.
        /// Выполняет операции атомарно провал или нет! 
        /// </summary>
        public async Task<CommandResult> Transfer(
             Money transferAmount, 
             Guid recieverWalletId, 
             Guid senderWalletId, 
             IFeePolicy? feePolicy, 
             int? freezeForDays)
        {
            var recieverWallet = await _dbContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == recieverWalletId);
            var senderWallet = await _dbContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(y => y.Id == senderWalletId);

            if (recieverWallet == null || senderWallet == null)
            {
                return CommandResult.NotFound("Не найден кошелек получателя или начальный кошелёк", recieverWalletId); 
            }

            senderWallet.Transfer(transferAmount, recieverWallet, feePolicy, freezeForDays); 

            return CommandResult.Success(); 
        }

        public async Task<CommandResult> Refund(Guid transactionId)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == transactionId);

            if (transaction == null)
                return CommandResult.NotFound("Не найдена транзакция", transactionId);

            if (transaction.AntoganistTransactionId == null)
            {
                // то есть возврат средств на какие то фентифлющки по типу випки
                // но такое не допускается так что нет! 
                throw new NotImplementedException("Подожди");
            } if (transaction.AntoganistTransactionId == transactionId)
            {
                throw new DomainException("Что то пошло очень не так, коррупция данных про транзакции");
            }

            var antoganistTx = await _dbContext.Transactions.FirstOrDefaultAsync(y => y.Id == transaction.AntoganistTransactionId);

            if (antoganistTx == null)
                return CommandResult.NotFound("Че бля", (Guid)transaction.AntoganistTransactionId);

            var returnToWallet = await _dbContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == antoganistTx.WalletId);
            var fromWallet = await _dbContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == transaction.WalletId);

            if (returnToWallet == null || fromWallet == null)
                return CommandResult.NotFound("Кошёлек не найден", antoganistTx.WalletId);

            // может просто упасть и все
            fromWallet.RefundTransaction(transaction, returnToWallet);

            return CommandResult.Success(fromWallet.Id); 
        }

        public async Task<CommandResult> BalanceOperation(Money amount, Guid walletId, BalanceOperationType operation)
        {
            // просто добавляет или убавляет деньги через транзакцию 
            var wallet = await _dbContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == walletId);

            if (wallet == null)
                return CommandResult.NotFound("Кошёлек не найден", walletId);

            wallet.BalanceOperation(amount, operation); 

            return CommandResult.Success(walletId);
        }

        // used to confirm gateway transaction deposit 
        public async Task<CommandResult> ConfirmTransaction(Guid txId)
        {
            var transaction = await _dbContext.Transactions
                .Include(x => x.Props)
                .FirstOrDefaultAsync(x => x.Id == txId);

            if (transaction == null)
                return CommandResult.NotFound("Транзакция не найдена", txId);

            if (transaction.Status != TransactionStatus.Pending)
                return CommandResult.UnprocessableEntity("Транзакция уже подтверждена или отменена.");

            var wallet = await _dbContext.Wallets
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Id == transaction.WalletId);

            Guard.Against.Null(wallet, message: "Кошелек не найден.");

            // Применяем транзакцию к кошельку
            wallet.AcceptTransaction(transaction);

            // Меняем статус транзакции
            transaction.Complete();

            return CommandResult.Success(transaction.Id);
        }

        public async Task<CommandResult> UnfreezeAmount(Money amount, Guid walletId)
        {
            return await BalanceOperation(amount, walletId, BalanceOperationType.Unfreeze); 
        }


        public async Task<CommandResult> FreezeAmount(Money amount, Guid walletId, int frozenTime)
        {
            return await BalanceOperation(amount, walletId, BalanceOperationType.Freeze);
        }
    }
}
