using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class TransactionCreated(TransactionEntity transaction) : DomainEvent
    {
        public TransactionEntity Transaction { get; set; } = transaction;
    }

    public class TransactionConfirmed(TransactionEntity transaction) : DomainEvent
    {
        public TransactionEntity Transaction { get; set; } = transaction;
    }

    public class TransactionRolledBack : DomainEvent
    {
        public Guid WalletId { get; }
        public Guid TransactionId { get; }
        public Money Amount { get; }
        public TransactionFlow Flow { get; }
        public DateTime OccurredOn { get; }

        public TransactionRolledBack(WalletEntity wallet, TransactionEntity transaction)
        {
            WalletId = wallet.Id;
            TransactionId = transaction.Id;
            Amount = transaction.RawAmount;
            Flow = transaction.Flow;
            OccurredOn = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"[TransactionRolledBack] WalletId: {WalletId}, TransactionId: {TransactionId}, Amount: {Amount.Amount}, Flow: {Flow}, Date: {OccurredOn}";
    }

    public class TransactionAddedEvent : DomainEvent
    {
        public Guid WalletId { get; }
        public Guid TransactionId { get; }
        public Money Amount { get; }
        public TransactionFlow Flow { get; }
        public DateTime OccurredOn { get; }

        public TransactionAddedEvent(TransactionEntity transaction)
        {
            WalletId = transaction.WalletId;
            TransactionId = transaction.Id;
            Amount = transaction.RawAmount;
            Flow = transaction.Flow;
            OccurredOn = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"[TransactionAddedEvent] WalletId: {WalletId}, TransactionId: {TransactionId}, Amount: {Amount.Amount}, Flow: {Flow}, Date: {OccurredOn}";
    }

    public class TransactionRefundedWithoutReceiver : DomainEvent
    {
        public Guid WalletId { get; }
        public Guid TransactionId { get; }
        public Money Amount { get; }
        public DateTime OccurredOn { get; }

        public TransactionRefundedWithoutReceiver(WalletEntity wallet, TransactionEntity refundTransaction)
        {
            WalletId = wallet.Id;
            TransactionId = refundTransaction.Id;
            Amount = refundTransaction.RawAmount;
            OccurredOn = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"[TransactionRefundedWithoutReceiver] WalletId: {WalletId}, " +
            $"TransactionId: {TransactionId}, Amount: {Amount.Amount}, Date: {OccurredOn}";
    }

}
