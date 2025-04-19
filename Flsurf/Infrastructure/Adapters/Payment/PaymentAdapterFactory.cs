using Flsurf.Infrastructure.Adapters.Payment.Systems;

namespace Flsurf.Infrastructure.Adapters.Payment
{
    public class PaymentAdapterFactory : IPaymentAdapterFactory
    {
        private readonly Dictionary<PaymentProviders, IPaymentAdapter> Adapters = new();

        public PaymentAdapterFactory(IHttpClientFactory httpClient, IConfiguration config)
        {
            Adapters.Add(
                PaymentProviders.BankCardRu,
                new StripePaymentAdapter(
                    httpClient.CreateClient("stripe"),
                    new StripeConfig() { 
                        SecretKey = config["Payments:Stripe:SecretKey"] 
                            ?? throw new NullReferenceException("Нету secretkey для страйпа")
                    }
                )
            ); 
            Adapters.Add(PaymentProviders.Test, new TestPaymentAdapter());
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
