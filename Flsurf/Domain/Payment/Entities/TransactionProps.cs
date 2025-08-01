﻿using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Policies;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Domain.Payment.Entities
{
    [Owned]
    public class TransactionPropsEntity : ValueObject
    {
        public string? PaymentUrl { get; set; } = string.Empty;
        public string? SuccessUrl { get; set; } = string.Empty;
        public string PaymentGateway { get; set; } = string.Empty;
        public string ProviderPaymentId { get; set; } = string.Empty; 
        public string? ClientSecret { get; set; } = string.Empty;
        public FeeContext FeeContext { get; set; } = default!; 

        public TransactionPropsEntity() { }  // Для EF CORE 

        public static TransactionPropsEntity CreateGatewayProps(
            string paymentUrl,
            string successUrl,
            string paymentGateway,
            FeeContext feeContext, 
            string providerPaymentId)
        {
            return new TransactionPropsEntity
            {
                PaymentUrl = paymentUrl,
                SuccessUrl = successUrl,
                PaymentGateway = paymentGateway,
                FeeContext = feeContext,
                ProviderPaymentId = providerPaymentId, 
            };
        }

        public void SetProviderPaymentId(string paymentId) => ProviderPaymentId = paymentId; 

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PaymentUrl;
            yield return SuccessUrl;
            yield return PaymentGateway;
        }
    }
}
