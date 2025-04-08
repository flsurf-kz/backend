using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Policies;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Domain.Payment.Entities
{
    [Owned]
    public class TransactionPropsEntity : ValueObject
    {
        public string PaymentUrl { get; private set; } = string.Empty;
        public string SuccessUrl { get; private set; } = string.Empty;
        public string PaymentGateway { get; private set; } = string.Empty;
        public FeeContext FeeContext { get; private set; } = default!; 

        public TransactionPropsEntity() { }  // Для EF CORE 

        public static TransactionPropsEntity CreateGatewayProps(
            string paymentUrl,
            string successUrl,
            string paymentGateway,
            FeeContext feeContext)
        {
            return new TransactionPropsEntity
            {
                PaymentUrl = paymentUrl,
                SuccessUrl = successUrl,
                PaymentGateway = paymentGateway,
                FeeContext = feeContext
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PaymentUrl;
            yield return SuccessUrl;
            yield return PaymentGateway;
        }
    }
}
