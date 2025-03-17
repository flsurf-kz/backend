using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Events;
using Flsurf.Domain.Payment.Exceptions;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.Freelance.Entities;

namespace Flsurf.Domain.Payment.Entities
{
    public class PurchaseEntity : BaseAuditableEntity
    {
        [Required]
        public PurchaseType Type { get; private set; }

        [Required]
        public PurchaseStatus Status { get; private set; } = PurchaseStatus.Pending;

        [Required]
        public Money Amount { get; private set; }

        [Required]
        public Guid WalletId { get; private set; } // ID кошелька, с которого списываются средства

        // Дополнительные данные для конкретных типов покупок:

        // Для повышения заказа: ID заказа, который продвигается
        public Guid? RelatedOrderId { get; private set; }

        // Для премиум подписки: дата окончания подписки
        public DateTime? SubscriptionValidUntil { get; private set; }

        public string? Description { get; private set; }

        // Фабричный конструктор скрыт, используем статические методы для создания
        private PurchaseEntity(PurchaseType type, Money amount, Guid walletId, string? description = null)
        {
            Id = Guid.NewGuid();
            Type = type;
            Amount = amount;
            WalletId = walletId;
            Description = description;
            Status = PurchaseStatus.Pending;
        }

        // Фабричный метод для создания покупки повышения заказа
        public static PurchaseEntity CreateOrderPromotion(Money amount, Guid walletId, Guid relatedOrderId, string? description = null)
        {
            var purchase = new PurchaseEntity(PurchaseType.OrderPromotion, amount, walletId, description);
            purchase.RelatedOrderId = relatedOrderId;
            return purchase;
        }

        // Фабричный метод для создания покупки премиум подписки
        public static PurchaseEntity CreatePremiumSubscription(Money amount, Guid walletId, DateTime subscriptionValidUntil, string? description = null)
        {
            var purchase = new PurchaseEntity(PurchaseType.PremiumSubscription, amount, walletId, description);
            purchase.SubscriptionValidUntil = subscriptionValidUntil;
            return purchase;
        }

        // Метод для завершения покупки
        public void Complete(Guid transactionId)
        {
            if (Status != PurchaseStatus.Pending)
                throw new InvalidOperationException("Покупка уже завершена или отменена.");

            Status = PurchaseStatus.Completed;
            // Здесь можно сохранить идентификатор связанной транзакции, если требуется
            AddDomainEvent(new PurchaseCompletedEvent(this, transactionId));
        }

        // Метод для пометки покупки как неудачной с указанием причины
        public void Fail(string reason)
        {
            if (Status != PurchaseStatus.Pending)
                throw new InvalidOperationException("Покупка уже завершена или отменена.");

            Status = PurchaseStatus.Failed;
            Description = reason;
            AddDomainEvent(new PurchaseFailedEvent(this, reason));
        }

        // Метод для отмены покупки с указанием причины
        public void Cancel(string reason)
        {
            if (Status != PurchaseStatus.Pending)
                throw new InvalidOperationException("Покупка уже завершена или отменена.");

            Status = PurchaseStatus.Canceled;
            Description = reason;
            AddDomainEvent(new PurchaseCanceledEvent(this, reason));
        }
    }
}
