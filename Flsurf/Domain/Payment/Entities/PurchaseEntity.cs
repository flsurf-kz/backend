using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Events;
using Flsurf.Domain.Payment.Exceptions;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Domain.Payment.Entities
{
    public class PurchaseEntity : BaseAuditableEntity
    {
        // Assuming there's an enum for Currency that was not included in the initial schema
        [Required]
        public CurrencyEnum Currency { get; set; }

        [Required]
        public Money Amount { get; init; } = new(0);

        [Required]
        public WalletEntity Wallet { get; init; } = null!;

        [Required]
        public bool Completed { get; private set; } = false;

        [Required]
        public PurchaseStatus Status { get; set; }
        [ForeignKey(nameof(ReviewEntity))]
        public Guid? ReviewId { get; private set; }

        [Required]
        [ForeignKey(nameof(CourseEntity))]
        public Guid CourseId { get; init; } 

        [Required]
        public TransactionEntity Transaction { get; private set; } = null!;
        [Required]
        // required for two step confirmation
        public bool Confirmed { get; private set; } = false; 

        [MaxLength(256)]
        public string? StatusDescription { get; set; } = null!;
        [Required]
        public UserEntity CreatedBy { get; set; } = null!; 

        public static PurchaseEntity Create(WalletEntity wallet, TransactionEntity transaction, CourseEntity course)
        {
            if (wallet.User == null)
                throw new ArgumentException("Wallet entity does not have user value, load it!"); 
            var purchase = new PurchaseEntity
            {
                Wallet = wallet,
                CourseId = course.Id,
                Amount = transaction.Amount,
                Currency = transaction.Amount.Currency,
                Transaction = transaction,
                Status = PurchaseStatus.Processing, 
                CreatedBy = wallet.User, 
            };

            purchase.AddDomainEvent(new PurchaseCreated(purchase));

            return purchase;
        }

        public void Problem(string description)
        {
            Status = PurchaseStatus.Rejected;
            StatusDescription = description;
        }

        public void Complete(ReviewEntity review)
        {
            if (Status == PurchaseStatus.Success || Completed)
                throw new PurchaseIsAlreadyCompleted(this);

            Completed = true;
            ReviewId = review.Id;
            Status = PurchaseStatus.Success;
        }

        public void Confirm()
        {
            if (Status != PurchaseStatus.Success)

            Confirmed = true;

            AddDomainEvent(new PurchaseConfirmed(this)); 
        }
    }
}
