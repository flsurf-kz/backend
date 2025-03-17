using Flsurf.Application.Common.Exceptions;
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

        // Загрузка всей коллекции идет только при операциях с кошёлкем 
        [JsonIgnore]
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
            AddDomainEvent(new TransactionAddedEvent(transaction));
        }

        public void AcceptTransaction(TransactionEntity transaction)
        {
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
        public void Transfer(
            Money transferMoney, 
            WalletEntity recieverWallet,
            IFeePolicy? feePolicy, 
            int? freezeFundsDays)
        {
            EnsureNotBlocked();

            AcceptTransaction(TransactionEntity.Create(
                Id,
                transferMoney,
                feePolicy ?? new NoPolicy(), 
                TransactionType.Transfer,
                TransactionFlow.Outgoing));

            recieverWallet.AcceptTransaction(TransactionEntity.CreateFrozen(
                recieverWallet.Id, 
                transferMoney, 
                feePolicy ?? new NoPolicy(), 
                TransactionType.Transfer, 
                TransactionFlow.Incoming,
                freezeFundsDays ?? 2)); 
        }

        // ✅ Пополнение счета
        private void Deposit(Money amount)
        {
            EnsureNotBlocked();
            AvailableBalance += amount;
            AddDomainEvent(new WalletBalanceIncreased(this, amount));
        }

        // ✅ Списание средств
        private void Withdraw(Money amount)
        {
            EnsureNotBlocked();
            if (AvailableBalance < amount)
                throw new NotEnoughMoneyException(Id);

            AvailableBalance -= amount;
            AddDomainEvent(new WalletBalanceDecreased(this, amount));
        }

        // 🔒 Заморозка средств
        private void FreezeAmount(Money amount, DateTime frozenUntil)
        {
            EnsureNotBlocked();
            if (AvailableBalance < amount)
                throw new NotEnoughMoneyException(Id);

            Frozen += amount;
            AvailableBalance -= amount;
            // добавить потом таску которая размараживает деньги 
            AddDomainEvent(new WalletAmountFrozen(this, amount, frozenUntil));
        }

        // ❄️ Размораживание средств без контекста 
        private void UnfreezeAmount(Money amount) 
        {
            EnsureNotBlocked();

            if (Frozen < amount)
                throw new ArgumentException("Нельзя разморозить больше средств, чем было заморожено");

            Frozen -= amount;
            AvailableBalance += amount;
            AddDomainEvent(new WalletUnfrozenAmount(this, amount));
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
            AddDomainEvent(new WalletBlocked(this, reason.ToString()));
        }

        // ❌ Откат транзакции создает еще одну двух сторонную транзакцию которая берет деньги из другого кошелка и сует в другой 
        public void RollbackTransaction(TransactionEntity transaction, WalletEntity returnTo)
        {
            EnsureNotBlocked();

            AcceptTransaction(new TransactionEntity(
                Id,
                transaction.NetAmount,
                new NoPolicy(),
                TransactionType.Rollback,
                TransactionFlow.Outgoing,
                null,
                null,
                "Отказ транзакции"));

            returnTo.AcceptTransaction(new TransactionEntity(
                returnTo.Id,
                transaction.NetAmount, 
                new NoPolicy(),  // TODO!! 
                TransactionType.Rollback, 
                TransactionFlow.Incoming, 
                null, 
                null, 
                "Отказ транзакции"));

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
