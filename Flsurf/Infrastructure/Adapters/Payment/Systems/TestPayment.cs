namespace Flsurf.Infrastructure.Adapters.Payment.Systems
{
    public class TestPaymentAdapter : IPaymentAdapter
    {
        public TestPaymentAdapter() { }

        // Инициирует платеж и возвращает результат операции.
        // В параметры можно включить сумму, валюту, описание платежа и дополнительные параметры, 
        // специфичные для каждой платежной системы.
        public Task<InitPaymentResult> InitPayment(PaymentInitRequest req)
        {
            return Task.FromResult(new InitPaymentResult(true, "", "", null, null, null) { }); 
        }


        // Метод для проверки статуса платежа, если это требуется по логике платежной системы.
        // Может возвращать статус платежа в удобочитаемом формате.
        public Task<PaymentStatus> GetStatusAsync(string providerPaymentId)
        {
            return Task.FromResult(PaymentStatus.Pending); 
        }

        public Task<PayoutInitResult> InitPayoutAsync(PayoutInitRequest req)
        {
            return Task.FromResult(new PayoutInitResult(Success: true, ProviderPayoutId: req.ExternalAccountId) { });
        }
        
        // Опциональный метод для возврата средств, если такая возможность поддерживается платежной системой.
        // Возвращает результат операции возврата.
        public Task<RefundResult> RefundAsync(string providerPaymentId,
                                       decimal amount)
        {
            return Task.FromResult(new RefundResult(true, ""));
        }

        public Task<CardMeta?> FetchCardMetaAsync(string paymentMethodToken)
        {
            return Task.FromResult(new CardMeta("kaspi", "4444", 10, 26) ?? null); 
        }


        public Task<CardSetupDetails> PrepareCardSetupAsync(PrepareCardSetupRequest request)
        {
            return Task.FromResult(
                new CardSetupDetails() { 
                    ClientSecretForWidget = "", 
                    ProviderSetupId = "", 
                    Success = true });
        }
    }
}
