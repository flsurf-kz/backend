using Flsurf.Application.Common.cqrs;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.Adapters.Permissions;
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

        public async Task<CommandResult> Rollback(Guid transactionId)
        {

        }
    }

}
