using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Events;
using Flsurf.Domain.Payment.Exceptions;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public CurrencyEnum Currency { get; private set; } = CurrencyEnum.RussianRuble;

        [Required]
        public Money Frozen { get; private set; } = new(0, CurrencyEnum.RussianRuble);

        [Required]
        public Money AvailableBalance { get; private set; } = new(1000, CurrencyEnum.RussianRuble);

        [Required]
        public Money PendingIncome { get; private set; } = new(0, CurrencyEnum.RussianRuble);

        [Required]
        public bool Blocked { get; private set; } = false;

        public WalletBlockReason BlockReason { get; private set; } = WalletBlockReason.None;

        // 🔥 Коллекция связанных транзакций
        public ICollection<TransactionEntity> Transactions { get; private set; } = new List<TransactionEntity>();

        private const int MaxTransactionsToTrack = 10; // Ограничение для избежания перегрузки коллекции

        public static WalletEntity Create(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            var wallet = new WalletEntity
            {
                User = user,
                UserId = user.Id,
                Currency = CurrencyEnum.RussianRuble,
                Frozen = new Money(0, CurrencyEnum.RussianRuble),
                AvailableBalance = new Money(1000, CurrencyEnum.RussianRuble),
                PendingIncome = new Money(0, CurrencyEnum.RussianRuble),
                Blocked = false
            };

            wallet.AddDomainEvent(new WalletCreated(wallet));
            return wallet;
        }

        // 🔁 Перевод между кошельками
        public void TransferTo(WalletEntity recipientWallet, Money amount)
        {
            if (AvailableBalance < amount)
                throw new InvalidOperationException("Недостаточно средств для перевода.");

            this.Withdraw(amount);
            recipientWallet.Deposit(amount);

            AddDomainEvent(new FundsTransferredEvent(this, recipientWallet, amount)); 
        }

        // ✅ Переопределение добавления транзакций (безопасный способ)
        public void AddTransaction(TransactionEntity transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (Transactions.Any(t => t.Id == transaction.Id))
                throw new InvalidOperationException("Транзакция уже добавлена!");

            if (Transactions.Count >= MaxTransactionsToTrack)
                Transactions.Remove(Transactions.First()); // Удаляем самую старую транзакцию

            Transactions.Add(transaction);
            AddDomainEvent(new TransactionAddedEvent(transaction));
        }

        // ✅ Подтверждение двухсторонней транзакции
        public void ConfirmTransaction(TransactionEntity transaction, WalletEntity fromWallet)
        {
            EnsureNotBlocked();

            VerifyTransaction(transaction, true);

            if (transaction.Flow == TransactionFlow.Outgoing)
            {
                fromWallet.Deposit(transaction.Amount);
                this.Withdraw(transaction.Amount);
            }
            else if (transaction.Flow == TransactionFlow.Incoming)
            {
                fromWallet.Withdraw(transaction.Amount);
                this.Deposit(transaction.Amount);
            }

            AddTransaction(transaction);  // Сохраняем в коллекции у обеих сторон
            fromWallet.AddTransaction(transaction);

            AddDomainEvent(new ConfirmedTransaction(this, transaction, fromWallet));
            transaction.Complete(); 
        }

        // ✅ Пополнение счета
        public void Deposit(Money amount)
        {
            EnsureNotBlocked();
            AvailableBalance += amount;
            AddDomainEvent(new WalletBalanceIncreased(this, amount));
        }

        // ✅ Списание средств
        public void Withdraw(Money amount)
        {
            EnsureNotBlocked();
            if (AvailableBalance < amount)
                throw new NotEnoughMoneyException(Id);

            AvailableBalance -= amount;
            AddDomainEvent(new WalletBalanceDecreased(this, amount));
        }

        // 🔒 Заморозка средств
        public void FreezeAmount(Money amount)
        {
            EnsureNotBlocked();
            if (AvailableBalance < amount)
                throw new NotEnoughMoneyException(Id);

            Frozen += amount;
            AvailableBalance -= amount;
            AddDomainEvent(new WalletAmountFrozen(this, amount));
        }

        // ❄️ Размораживание средств
        public void UnfreezeAmount(Money amount)
        {
            EnsureNotBlocked();
            if (Frozen < amount)
                throw new ArgumentException("Нельзя разморозить больше средств, чем было заморожено");

            Frozen -= amount;
            AvailableBalance += amount;
            AddDomainEvent(new WalletUnfrozenAmount(this, amount));
        }

        // ❗️ Блокировка кошелька с указанием причины
        public void Block(WalletBlockReason reason)
        {
            if (Blocked)
                throw new WalletIsBlocked(Id, reason);

            Blocked = true;
            BlockReason = reason;
            AddDomainEvent(new WalletBlocked(this, reason.ToString()));
        }

        // ❌ Откат транзакции
        public void RollbackTransaction(TransactionEntity transaction)
        {
            EnsureNotBlocked();
            VerifyTransaction(transaction, true);

            if (transaction.Flow == TransactionFlow.Incoming)
                AvailableBalance -= transaction.Amount;
            else if (transaction.Flow == TransactionFlow.Outgoing)
                AvailableBalance += transaction.Amount;

            AddDomainEvent(new TransactionRolledBack(this, transaction));
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
