using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Payment.Entities
{
    public class UserPaymentGatewayCustomer : BaseAuditableEntity // Или BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; } = null!;

        // Ссылка на TransactionProviderEntity, чтобы знать, к какому провайдеру относится CustomerId
        // Например, у одного пользователя может быть ID для Stripe и ID для PayPal.
        [ForeignKey(nameof(PaymentProvider))]
        public Guid PaymentProviderId { get; set; }
        public virtual TransactionProviderEntity PaymentProvider { get; set; } = null!;

        /// <summary>
        /// Идентификатор клиента на стороне платежного шлюза (например, Stripe Customer ID "cus_XXXXXXXX").
        /// </summary>
        public string CustomerIdInProvider { get; set; } = string.Empty;

        // Можно добавить другие специфичные для связки данные, если потребуется
        // public bool IsActive { get; set; } = true;
        // public DateTime? LastVerifiedAt { get; set; }

        public static UserPaymentGatewayCustomer Create(Guid userId, Guid paymentProviderId, string customerIdInProvider)
        {
            // Валидация входных данных
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (paymentProviderId == Guid.Empty) throw new ArgumentNullException(nameof(paymentProviderId));
            if (string.IsNullOrWhiteSpace(customerIdInProvider)) throw new ArgumentNullException(nameof(customerIdInProvider));

            return new UserPaymentGatewayCustomer
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PaymentProviderId = paymentProviderId,
                CustomerIdInProvider = customerIdInProvider
            };
        }
    }
}
