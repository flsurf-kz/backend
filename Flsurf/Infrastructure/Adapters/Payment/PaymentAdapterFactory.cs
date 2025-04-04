﻿using Flsurf.Infrastructure.Adapters.Payment.Systems;

namespace Flsurf.Infrastructure.Adapters.Payment
{
    public class PaymentAdapterFactory : IPaymentAdapterFactory
    {
        private readonly Dictionary<PaymentProviders, IPaymentAdapter> Adapters = new();

        public PaymentAdapterFactory(IHttpClientFactory httpClient)
        {
            // Регистрация доступных адаптеров
            Adapters.Add(PaymentProviders.BankCardRu, new PayPalychPaymentAdapter(
                httpClient.CreateClient("paypalych"),
                new PayPalychConfiguration()
                {
                    ShopId = "1234567",
                    HostUrl = "lol",
                    SuccessUrl = "/api/payment/payout/paypalych"
                }
            ));
            //Adapters.Add(PaymentProviders.Balance, new BalancePaymentAdapter());
            Adapters.Add(PaymentProviders.Test, new TestPaymentAdapter());
            // Добавьте другие адаптеры здесь
        }

        public IPaymentAdapter GetPaymentAdapter(string providerId)
        {
            if (!Enum.TryParse(providerId, out PaymentProviders provider))
            {
                throw new ArgumentException($"{providerId} is not convertable to Payment providers");
            }

            if (Adapters.TryGetValue(provider, out var adapter))
            {
                return adapter;
            }

            throw new ArgumentException($"Payment adapter for provider ID {providerId} not found.", nameof(providerId));
        }
    }
}
