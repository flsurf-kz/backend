using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Payment.Entities
{
    public class TransactionEntity : BaseAuditableEntity
    {
        [ForeignKey(nameof(WalletEntity))]
        public Guid WalletId { get; private set; }
        public Guid? AntoganistTransactionId { get; set; }
        public Money RawAmount { get; private set; }
        public Money NetAmount { get; private set; }
        public Money AppliedFee { get; private set; }
        public TransactionStatus Status { get; private set; } = TransactionStatus.Pending; 
        public TransactionType Type { get; private set; }
        public TransactionFlow Flow { get; private set; }  // 🔥 Новое поле
        public TransactionPropsEntity? Props { get; private set; } = null!;
        public DateTime? FrozenUntil { get; private set; } 
        public string? Comment { get; private set; }
        public DateTime? CompletedAt { get; set; }

        public TransactionEntity(
            Guid walletId,
            Money amount,
            IFeePolicy feePolicy,
            TransactionType type,
            TransactionFlow flow,  // -> Новое поле
            TransactionPropsEntity? props,
            int? freezeTimeInDays, 
            string? comment)
        {
            WalletId = walletId;
            RawAmount = amount;

            AppliedFee = feePolicy.CalculateFee(amount, type, props?.FeeContext);
            NetAmount = RawAmount - AppliedFee;

            Type = type;
            Flow = flow;  // ➔ Указание направления денег
            Status = TransactionStatus.Pending;
            Comment = comment; 

            Props = props ?? throw new ArgumentNullException(nameof(props));
            FrozenUntil = freezeTimeInDays != null ? DateTime.UtcNow.AddDays((double)freezeTimeInDays) : null;  
        }

        public static TransactionEntity Create(
            Guid walletId, 
            Money amount,
            IFeePolicy feePolicy, 
            TransactionType type,
            TransactionFlow flow)
        {
            return new TransactionEntity(walletId, amount, feePolicy, type, flow, null, null, null);  
        }

        public static TransactionEntity CreateFrozen(
            Guid walletId,
            Money amount,
            IFeePolicy feePolicy,
            TransactionType type,
            TransactionFlow flow, 
            int freezeDays)
        {
            return new TransactionEntity(walletId, amount, feePolicy, type, flow, null, freezeDays, null);
        }

        public bool IsIncoming() => Flow == TransactionFlow.Incoming;
        public bool IsOutgoing() => Flow == TransactionFlow.Outgoing;
        public bool IsInternal() => Flow == TransactionFlow.Internal;

        public void Complete()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not in a valid state to complete.");

            Status = TransactionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not in a valid state to cancel.");

            Status = TransactionStatus.Cancelled;
        }
    }
}
