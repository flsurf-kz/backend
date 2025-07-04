﻿using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Events;
using Flsurf.Domain.Payment.Exceptions;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Entities
{
    public class WalletEntity : BaseAuditableEntity
    {
        [ForeignKey("User")]
        [Required]
        public Guid UserId { get; private set; }

        [Required]
        public virtual UserEntity User { get; private set; } = null!;

        [Required]
        public CurrencyEnum Currency { get; private set; } = CurrencyEnum.RUB;

        [Required]
        public Money Frozen { get; private set; } = new(0, CurrencyEnum.RUB);

        [Required]
        public Money AvailableBalance { get; private set; } = new(1000, CurrencyEnum.RUB);

        [Required]
        public Money PendingIncome { get; private set; } = new(0, CurrencyEnum.RUB);

        [Required]
        public bool Blocked { get; private set; } = false;

        public WalletBlockReason BlockReason { get; private set; } = WalletBlockReason.None;

        // Загрузка всей коллекции идет только при операциях с кошёлкем 
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<TransactionEntity> Transactions { get; private set; } = new List<TransactionEntity>();

        //[Timestamp]
        //public byte[]? RowVersion { get; private set; } = default; 


        public static WalletEntity Create(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            var wallet = new WalletEntity
            {
                User = user,
                UserId = user.Id,
                Currency = CurrencyEnum.RUB,
                Frozen = new Money(0, CurrencyEnum.RUB),
                AvailableBalance = new Money(1000, CurrencyEnum.RUB),
                PendingIncome = new Money(0, CurrencyEnum.RUB),
                Blocked = false, 
            };

            wallet.AddDomainEvent(new WalletCreated(wallet.Id));
            return wallet;
        }

        // ✅ Переопределение добавления транзакций (безопасный способ)
        private void AddTransaction(TransactionEntity transaction)
        {
            if (Transactions == null)
            {
                throw new NullReferenceException("Не загрузил транзакции пидр");
            }

            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (Transactions.Any(t => t.Id == transaction.Id))
                return;  // игнорировтаь 

            Transactions.Add(transaction);
            AddDomainEvent(new TransactionAddedEvent(transaction.WalletId, transaction.Id, transaction.RawAmount, transaction.Flow));
        }

        public void AcceptTransaction(TransactionEntity transaction)
        {
            if (transaction.WalletId != Id)
                throw new DomainException("Эта транзакция не для вашего кошелёка"); 

            EnsureNotBlocked(); 

            if (transaction.Flow == TransactionFlow.Outgoing)
                Withdraw(transaction.NetAmount);

            if (transaction.Flow == TransactionFlow.Incoming)
            {

                if (transaction.FrozenUntil != null)
                {
                    FreezeAmount(transaction.NetAmount, (DateTime)transaction.FrozenUntil);
                } else
                {
                    // депозит и передача денег контрактом это другие вещи 
                    Deposit(transaction.NetAmount); 
                }
            }

            transaction.Complete();

            AddTransaction(transaction); 
        }

        // ✅ Подтверждение двухсторонней транзакции
        public Tuple<TransactionEntity, TransactionEntity> Transfer(
            Money transferMoney, 
            WalletEntity recieverWallet,
            IFeePolicy? feePolicy, 
            int? freezeFundsDays)
        {
            EnsureNotBlocked();

            var thisWalletTx = TransactionEntity.Create(
                Id,
                transferMoney - 0,
                feePolicy ?? new NoFeePolicy(),
                TransactionType.Transfer,
                TransactionFlow.Outgoing, 
                comment: "Трансфер");

            var recieverTx = TransactionEntity.CreateFrozen(
                recieverWallet.Id,
                transferMoney - 0,
                feePolicy ?? new NoFeePolicy(),
                TransactionType.Transfer,
                TransactionFlow.Incoming,
                freezeFundsDays ?? 2); 

            // reciever rolls back!! 
            recieverTx.AntoganistTransactionId = thisWalletTx.Id;
            thisWalletTx.AntoganistTransactionId = recieverTx.Id;

            AcceptTransaction(thisWalletTx);

            recieverWallet.AcceptTransaction(recieverTx); 

            return new(thisWalletTx, recieverTx); 
        }

        public TransactionEntity? BalanceOperation(Money amount, BalanceOperationType type)
        {
            EnsureNotBlocked();

            if (type == BalanceOperationType.Freeze)
            {
                FreezeAmount(amount, DateTime.UtcNow.AddDays(4)); // потому что лень делать заморозку 
            }
            else if (type == BalanceOperationType.Unfreeze)
            {
                UnfreezeAmount(amount);
            }
            else if (type == BalanceOperationType.PendingIncome)
            {
                PendingIncome += amount;
            }
            else
            {
                var tx = TransactionEntity.Create(
                    walletId: Id,
                    amount: amount,
                    feePolicy: new NoFeePolicy(),
                    type: TransactionType.Transfer,
                    flow: type == BalanceOperationType.Deposit ? TransactionFlow.Incoming : TransactionFlow.Outgoing,
                    comment: "Пополнение баланса"); 

                // admin balance change! 
                AcceptTransaction(tx);
                return tx;
            }
            return null; 
        }

        // ✅ Пополнение счета
        private void Deposit(Money amount)
        {
            EnsureNotBlocked();
            AvailableBalance += amount;
            AddDomainEvent(new WalletBalanceIncreased(this.Id, amount));
        }

        // ✅ Списание средств
        private void Withdraw(Money amount)
        {
            EnsureNotBlocked();
            if (AvailableBalance < amount)
                throw new NotEnoughMoneyException(Id);

            AvailableBalance -= amount;
            AddDomainEvent(new WalletBalanceDecreased(this.Id, amount));
        }

        // 🔒 Заморозка средств
        private void FreezeAmount(Money amount, DateTime frozenUntil)
        {
            EnsureNotBlocked();
            if (AvailableBalance < amount)
                // морозится все деньги
                amount = AvailableBalance; 

            Frozen += amount;
            AvailableBalance -= amount;
            // добавить потом таску которая размараживает деньги 
            AddDomainEvent(new WalletAmountFrozen(this.Id, amount, frozenUntil));
        }

        // ❄️ Размораживание средств без контекста  
        private void UnfreezeAmount(Money amount) 
        {
            EnsureNotBlocked();

            if (Frozen < amount)
                amount = AvailableBalance; 

            Frozen -= amount;
            AvailableBalance += amount;
            AddDomainEvent(new WalletUnfrozenAmount(this.Id, amount));
        }

        public void UnfreezeByTransaction(TransactionEntity transaction, bool adminOverride = false)
        {
            EnsureNotBlocked();

            if (Transactions == null)
                throw new NullReferenceException("Загрузи транзакции пидр"); 

            if (!Transactions.Contains(transaction))
            {
                throw new DomainException("Нету такой транзакции"); 
            }

            if (DateTime.UtcNow > transaction.FrozenUntil && !adminOverride)
            {
                throw new DomainException("Эти средства все еще заморожены"); 
            }

            UnfreezeAmount(transaction.RawAmount); 
        }

        // ❗️ Блокировка кошелька с указанием причины
        public void Block(WalletBlockReason reason)
        {
            if (Blocked)
                throw new WalletIsBlocked(Id, reason);

            Blocked = true;
            BlockReason = reason;
            AddDomainEvent(new WalletBlocked(this.Id, reason.ToString()));
        }

        // ❌ Откат транзакции создает еще одну двух сторонную транзакцию которая берет деньги из другого кошелка и сует в другой 
        public Tuple<TransactionEntity, TransactionEntity> RefundTransaction(TransactionEntity transaction, WalletEntity returnTo)
        {
            EnsureNotBlocked();

            var outgoingTx = TransactionEntity.Create(
                walletId: Id,
                amount: transaction.NetAmount,
                feePolicy: new NoFeePolicy(),
                type: TransactionType.Refund,
                flow: TransactionFlow.Outgoing, 
                comment: "Откат транзакции"
            );

            var incomingTx = TransactionEntity.Create(
                walletId: returnTo.Id,
                amount: transaction.NetAmount,
                feePolicy: new NoFeePolicy(),
                type: TransactionType.Refund,
                flow: TransactionFlow.Incoming, 
                comment: "Возврат средств"
            );

            incomingTx.AntoganistTransactionId = outgoingTx.Id;
            outgoingTx.AntoganistTransactionId = incomingTx.Id;

            AcceptTransaction(outgoingTx);
            returnTo.AcceptTransaction(incomingTx);

            AddDomainEvent(new TransactionRolledBack(this.Id, transaction.Id, transaction.RawAmount, transaction.Flow));

            return new(outgoingTx, incomingTx); 
        }

        public TransactionEntity RefundTransactionWithoutReceiver(TransactionEntity transaction, IFeePolicy feePolicy)
        {
            EnsureNotBlocked();

            var refundTx = TransactionEntity.Create(
                walletId: Id,
                amount: transaction.NetAmount,
                feePolicy: feePolicy,
                type: TransactionType.Refund,
                flow: TransactionFlow.Outgoing,
                comment: "Возврат средств (без получателя)"
            );

            refundTx.AntoganistTransactionId = transaction.Id;

            AcceptTransaction(refundTx);

            AddDomainEvent(new TransactionRefundedWithoutReceiver(this, refundTx));

            return refundTx; 
        }


        // ✅ Проверка корректности транзакции
        public bool VerifyTransaction(TransactionEntity transaction, bool raiseErr = false)
        {
            bool fromUserWallet = transaction.WalletId == Id;
            bool createdByUserWallet = transaction.CreatedById == UserId;
            bool result = fromUserWallet || createdByUserWallet;

            if (!result && raiseErr)
                throw new TransactionVerificationFailed(Id, transaction.Id);

            return result;
        }

        // 🔍 Фиксация состояния счета (например, для аудита)
        public void RecordAmount() => AddDomainEvent(new RecordWallet(this));

        // ✅ Проверка на блокировку
        public void EnsureNotBlocked()
        {
            if (Blocked)
                throw new WalletIsBlocked(Id, BlockReason);
        }
    }
}
