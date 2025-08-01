﻿using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class TransactionCreated(TransactionEntity transaction) : DomainEvent
    {
        public Guid TransactionId { get; } = transaction.Id; 
    }

    public class TransactionConfirmed(TransactionEntity transaction) : DomainEvent
    {
        public Guid TransactionId { get; } = transaction.Id; 
    }

    public class TransactionRolledBack : DomainEvent
    {
        public Guid WalletId { get; }
        public Guid TransactionId { get; }
        public Money Amount { get; }
        public TransactionFlow Flow { get; }
        public DateTime OccurredOn { get; }

        public TransactionRolledBack(Guid walletId, Guid txId, Money amount, TransactionFlow flow)
        {
            WalletId = walletId;  
            TransactionId = txId;
            Amount = amount;
            Flow = flow;
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

        public TransactionAddedEvent(Guid walletId, Guid txId, Money rawAmount, TransactionFlow flow)
        {
            WalletId = walletId;
            TransactionId = txId;
            Amount = rawAmount;
            Flow = flow;
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
