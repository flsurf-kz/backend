namespace Flsurf.Infrastructure.Adapters.Payment
{
    public interface IPaymentAdapter
    {
        // Инициирует платеж и возвращает результат операции.
        // В параметры можно включить сумму, валюту, описание платежа и дополнительные параметры, 
        // специфичные для каждой платежной системы.
        Task<InitPaymentResult> InitPayment(PaymentInitRequest req);


        // Метод для проверки статуса платежа, если это требуется по логике платежной системы.
        // Может возвращать статус платежа в удобочитаемом формате.
        Task<PaymentStatus> GetStatusAsync(string providerPaymentId);

        // Опциональный метод для возврата средств, если такая возможность поддерживается платежной системой.
        // Возвращает результат операции возврата.
        Task<RefundResult> RefundAsync(string providerPaymentId,
                                       decimal amount);

        Task<CardMeta?> FetchCardMetaAsync(string paymentMethodToken);
    }
}
