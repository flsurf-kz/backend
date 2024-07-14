using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.Payment.Events;

namespace Flsurf.Domain.Payment.Entities
{
    public class TransactionEntity : BaseAuditableEntity
    {

        [Required]
        public Money Amount { get; set; } = null!;
        [Required]
        public TransactionOperations Operation { get; set; }

        [Required]
        public TransactionDirection Direction { get; set; }

        [Required]
        public TransactionStatusEnum Status { get; set; }

        [Required]
        public UserEntity CreatedByUser { get; set; } = null!;

        public DateTime? CompletedAt { get; set; }

        [Required]
        public Money Fee { get; set; } = null!;

        [Required]
        public TransactionProviderEntity Provider { get; set; } = null!;
        public TransactionPropsEntity? Props { get; set; }

        public static TransactionEntity Create(
            Money value,
            Money fee,
            TransactionOperations operation,
            TransactionDirection direction,
            TransactionProviderEntity provider,
            UserEntity user)
        {
            var transaction = new TransactionEntity
            {
                Id = Guid.NewGuid(),
                CreatedByUser = user,
                Fee = fee,
                Amount = value,
                Status = TransactionStatusEnum.Pending,
                Provider = provider,
                Direction = direction,
                Operation = operation
            };
            transaction.AddDomainEvent(new TransactionCreated(transaction));

            return transaction;
        }

        public void Confirm()
        {
            CompletedAt = DateTime.Now;
            Status = TransactionStatusEnum.Confirmed;
            AddDomainEvent(new TransactionConfirmed(this));
        }

        public void Complete(TransactionStatusEnum status)
        {
            CompletedAt = DateTime.Now;
            Status = status;
        }
    }
}
