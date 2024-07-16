using Flsurf.Domain.Payment.Events;
using Flsurf.Domain.Study.Entities;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Payment.Entities
{
    public class ReviewEntity : BaseAuditableEntity 
    {
        [ForeignKey(nameof(PurchaseEntity)), Required]
        public Guid PurchaseId { get; set; }
        [NotMapped]
        private int _rating;

        [Required]
        public int Rating
        {
            get => _rating;
            private set => _rating = value < 0 ? 0 : value > 5 ? 5 : value;
        }
        public string Text { get; set; } = null!;
        [ForeignKey(nameof(UserEntity))]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(CourseEntity))]
        public Guid CourseId { get; set; }

        public static ReviewEntity Create(Guid purchaseId, Guid userId, Guid courseId, int rating, string text)
        {
            var review = new ReviewEntity
            {
                PurchaseId = purchaseId,
                UserId = userId,
                CourseId = courseId,
                Rating = rating,
                Text = text
            };

            review.AddDomainEvent(new ReviewCreated(review));

            return review; 
        }
    }
}
