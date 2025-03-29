using Flsurf.Application.Common.Exceptions;
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

        public Money RawAmount { get; private set; } = null!;
        public Money NetAmount { get; private set; } = null!;
        public Money AppliedFee { get; private set; } = null!;

        public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;
        public TransactionType Type { get; private set; }
        public TransactionFlow Flow { get; private set; }

        public TransactionPropsEntity? Props { get; private set; }
        public DateTime? FrozenUntil { get; private set; }
        public string? Comment { get; private set; }
        public DateTime? CompletedAt { get; set; }

        public TransactionProviderEntity? Provider { get; set; }

        // EF Core constructor
        private TransactionEntity() { }

        public static TransactionEntity Create(
            Guid walletId,
            Money amount,
            IFeePolicy feePolicy,
            TransactionType type,
            TransactionFlow flow, 
            string? comment)
        {
            var tx = new TransactionEntity
            {
                WalletId = walletId,
                RawAmount = amount,
                AppliedFee = feePolicy.CalculateFee(amount, type, null),
                Type = type,
                Flow = flow,
                Status = TransactionStatus.Pending,
            };

            tx.NetAmount = tx.RawAmount - tx.AppliedFee;
            return tx;
        }

        public static TransactionEntity CreateFrozen(
            Guid walletId,
            Money amount,
            IFeePolicy feePolicy,
            TransactionType type,
            TransactionFlow flow,
            int freezeDays)
        {
            var tx = Create(walletId, amount, feePolicy, type, flow, null);
            tx.FrozenUntil = DateTime.UtcNow.AddDays(freezeDays);
            return tx;
        }

        public static TransactionEntity CreateWithProvider(
            Guid walletId,
            Money amount,
            TransactionFlow flow,
            TransactionType type,
            TransactionPropsEntity props,
            TransactionProviderEntity provider,
            IFeePolicy? feePolicy)
        {
            var tx = new TransactionEntity
            {
                WalletId = walletId,
                RawAmount = amount,
                AppliedFee = (feePolicy ?? new NoFeePolicy()).CalculateFee(amount, type, props?.FeeContext),
                Type = type,
                Flow = flow,
                Props = props,
                Provider = provider,
                Status = TransactionStatus.Pending,
                Comment = $"Транзакция в платежную систему"
            };

            tx.NetAmount = tx.RawAmount - tx.AppliedFee;
            return tx;
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

        public void ConfirmFromGateway()
        {
            if (Props == null)
                throw new DomainException("Не та транзакция для потверждения");

            Complete();
        }
    }
}
