using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Domain.Payment.Events
{
    public class ReviewCreated(ReviewEntity review) : DomainEvent
    {
        public ReviewEntity Review { get; set; }
    }
}
