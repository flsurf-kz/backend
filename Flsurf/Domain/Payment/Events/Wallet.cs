using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class WalletCreated(WalletEntity wallet) : DomainEvent
    {
        public WalletEntity Wallet { get; set; } = wallet;
    }

    public class BalanceChanged(WalletEntity wallet, Money amount) : DomainEvent
    {
        public WalletEntity Wallet { get; set; } = wallet;
        public Money Balance { get; set; } = amount;
    }

    public class RecordWallet(WalletEntity wallet) : DomainEvent
    {
        public WalletEntity Wallet { get; set; } = wallet;
    }

    public class WalletAmountFrozen(WalletEntity wallet, Money frozen, DateTime frozenUntil) : DomainEvent
    {
        public Money Frozen { get; set; } = frozen;
        public WalletEntity Wallet { get; set; } = wallet;
        public DateTime FrozenUntil { get; set; } = frozenUntil; 
    }

    public class WalletUnfrozenAmount(WalletEntity wallet, Money unfrozen) : DomainEvent
    {
        public WalletEntity Wallet { get; set; } = wallet;
        public Money Unfrozen { get; set; } = unfrozen;
    }

    public class WalletBlocked(WalletEntity wallet, string reason) : DomainEvent
    {
        public WalletEntity Wallet { get; set; } = wallet;
        public string Reason { get; set; } = reason;
    }

    public class ConfirmedTransaction(WalletEntity wallet, TransactionEntity transaction, WalletEntity fromWallet) : DomainEvent
    {
        public WalletEntity Wallet { get; set; } = wallet;
        public TransactionEntity Transaction { get; set; } = transaction;
        public WalletEntity FromWallet { get; set; } = fromWallet;
    }

    public class WalletBalanceDecreased(WalletEntity wallet, Money decreasedAmount) : DomainEvent
    {
        public Money DecreasedAmount { get; set; } = decreasedAmount;
        public WalletEntity Wallet { get; set; } = wallet;
    }

    public class WalletBalanceIncreased(WalletEntity wallet, Money increasedAmount) : DomainEvent
    {
        public Money IncreasedAmount { get; set; } = increasedAmount;
        public WalletEntity Wallet { get; set; } = wallet;
    }

    public class WalletUnfreezeAmount(WalletEntity wallet, Money unfrozen) : DomainEvent
    {
        public Money Unfrozen { get; set; } = unfrozen;
        public WalletEntity Wallet { get; set; } = wallet;
    }
}
