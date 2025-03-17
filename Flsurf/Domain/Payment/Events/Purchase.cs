using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Domain.Payment.Events
{
    public class PurchaseCreatedEvent(PurchaseEntity Purchase) : DomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        public override string ToString() =>
            $"[PurchaseCreatedEvent] PurchaseId: {Purchase.Id}, Type: {Purchase.Type}, Amount: {Purchase.Amount.Amount} {Purchase.Amount.Currency}, OccurredOn: {OccurredOn}";
    }

    public class PurchaseCompletedEvent(PurchaseEntity Purchase, Guid TransactionId) : DomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        public override string ToString() =>
            $"[PurchaseCompletedEvent] PurchaseId: {Purchase.Id}, TransactionId: {TransactionId}, OccurredOn: {OccurredOn}";
    }

    public class PurchaseFailedEvent(PurchaseEntity Purchase, string Reason) : DomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        public override string ToString() =>
            $"[PurchaseFailedEvent] PurchaseId: {Purchase.Id}, Reason: {Reason}, OccurredOn: {OccurredOn}";
    }

    public class PurchaseCanceledEvent(PurchaseEntity Purchase, string Reason) : DomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        public override string ToString() =>
            $"[PurchaseCanceledEvent] PurchaseId: {Purchase.Id}, Reason: {Reason}, OccurredOn: {OccurredOn}";
    }

}
