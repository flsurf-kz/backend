using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Domain.Payment.Entities
{
    public class TransactionEntity : BaseAuditableEntity
    {
        public Guid WalletId { get; private set; }
        public Money Amount { get; private set; }
        public Money NetAmount { get; private set; }
        public Money AppliedFee { get; private set; }
        public TransactionStatus Status { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionFlow Flow { get; private set; }  // 🔥 Новое поле
        public TransactionPropsEntity Props { get; private set; } = null!;

        public TransactionEntity(
            Guid walletId,
            Money amount,
            IFeePolicy feePolicy,
            TransactionType type,
            TransactionFlow flow,  // -> Новое поле
            TransactionPropsEntity props)
        {
            WalletId = walletId;
            Amount = amount;

            AppliedFee = feePolicy.CalculateFee(amount, type, props.FeeContext);
            NetAmount = Amount - AppliedFee;

            Type = type;
            Flow = flow;  // ➔ Указание направления денег
            Status = Enums.TransactionStatus.Pending;

            Props = props ?? throw new ArgumentNullException(nameof(props));
        }

        public bool IsIncoming() => Flow == TransactionFlow.Incoming;
        public bool IsOutgoing() => Flow == TransactionFlow.Outgoing;
        public bool IsInternal() => Flow == TransactionFlow.Internal;

        public void Complete()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not in a valid state to complete.");

            Status = TransactionStatus.Completed;
        }

        public void Cancel()
        {
            if (Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Transaction is not in a valid state to cancel.");

            Status = TransactionStatus.Cancelled;
        }
    }
}
