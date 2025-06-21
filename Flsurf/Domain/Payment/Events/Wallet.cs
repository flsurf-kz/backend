using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class WalletCreated(Guid walletId) : DomainEvent
    {
        public Guid WalletId { get; } = walletId; 
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

    public class WalletAmountFrozen(Guid walletId, Money frozen, DateTime frozenUntil) : DomainEvent
    {
        public Money Frozen { get; set; } = frozen;
        public Guid WalletId { get; set; } = walletId;
        public DateTime FrozenUntil { get; set; } = frozenUntil; 
    }

    public class WalletUnfrozenAmount(Guid walletId, Money unfrozen) : DomainEvent
    {
        public Guid WalletId { get; set; } = walletId;
        public Money Unfrozen { get; set; } = unfrozen;
    }

    public class WalletBlocked(Guid walletId, string reason) : DomainEvent
    {
        public Guid WalletId { get; set; } = walletId;
        public string Reason { get; set; } = reason;
    }

    public class ConfirmedTransaction(Guid walletId, Guid transactionId, Guid fromWalletId) : DomainEvent
    {
        public Guid WalletId { get; set; } = walletId; 
        public Guid TransactionId { get; set; } = transactionId;
        public Guid FromWalletId { get; set; } = fromWalletId;
    }

    public class WalletBalanceDecreased(Guid wallet, Money decreasedAmount) : DomainEvent
    {
        public Guid WalletId { get; } = wallet;
        public Money DecreasedAmount { get; } = decreasedAmount; 
    }

    public class WalletBalanceIncreased(Guid wallet, Money increasedAmount) : DomainEvent
    {
        public Guid WalletId { get; } = wallet;
        public Money IncreasedAmount { get; } = increasedAmount; 
    }

    public class WalletUnfreezeAmount(Guid wallet, Money unfrozen) : DomainEvent
    {
        public Guid WalletId { get; } = wallet;
        public Money Unfrozen { get; } = unfrozen; 
    }
}
