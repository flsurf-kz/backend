using Flsurf.Domain.Payment.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class PurchaseCreated(PurchaseEntity purchase) : BaseEvent
    {
        public PurchaseEntity Purchase { get; set; } = purchase;
    }

    public class PurchaseConfirmed(PurchaseEntity purchase) : DomainEvent {
        public PurchaseEntity Purchase { get; } = purchase;
    }
}
