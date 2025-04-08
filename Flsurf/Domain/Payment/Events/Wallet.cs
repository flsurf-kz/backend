using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class WalletCreated(WalletEntity wallet) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id; 
    }

    public class BalanceChanged(WalletEntity wallet, Money amount) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id;
        public Money Balance { get; } = amount; 
    }

    public class RecordWallet(WalletEntity wallet) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id; 
    }

    public class WalletAmountFrozen(WalletEntity wallet, Money frozen, DateTime frozenUntil) : DomainEvent
    {
        public Money Frozen { get; set; } = frozen;
        public Guid WalletId { get; set; } = wallet.Id;
        public DateTime FrozenUntil { get; set; } = frozenUntil; 
    }

    public class WalletUnfrozenAmount(WalletEntity wallet, Money unfrozen) : DomainEvent
    {
        public Guid WalletId { get; set; } = wallet.Id;
        public Money Unfrozen { get; set; } = unfrozen;
    }

    public class WalletBlocked(WalletEntity wallet, string reason) : DomainEvent
    {
        public Guid WalletId { get; set; } = wallet.Id;
        public string Reason { get; set; } = reason;
    }

    public class ConfirmedTransaction(WalletEntity wallet, TransactionEntity transaction, WalletEntity fromWallet) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id; 
        public Guid TransactionId { get; } = transaction.Id;
        public Guid FromWalletId { get; } = fromWallet.Id;
    }

    public class WalletBalanceDecreased(WalletEntity wallet, Money decreasedAmount) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id;
        public Money DecreasedAmount { get; } = decreasedAmount; 
    }

    public class WalletBalanceIncreased(WalletEntity wallet, Money increasedAmount) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id;
        public Money IncreasedAmount { get; } = increasedAmount; 
    }

    public class WalletUnfreezeAmount(WalletEntity wallet, Money unfrozen) : DomainEvent
    {
        public Guid WalletId { get; } = wallet.Id;
        public Money Unfrozen { get; } = unfrozen; 
    }
}
