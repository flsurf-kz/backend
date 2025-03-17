﻿using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Policies;

namespace Flsurf.Domain.Payment.Entities
{
    public class TransactionPropsEntity : ValueObject
    {
        public string PaymentUrl { get; private set; }
        public string SuccessUrl { get; private set; }
        public string PaymentGateway { get; private set; }
        public FeeContext FeeContext { get; private set; }

        private TransactionPropsEntity() { }

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
