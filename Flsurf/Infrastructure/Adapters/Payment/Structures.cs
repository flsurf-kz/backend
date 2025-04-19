namespace Flsurf.Infrastructure.Adapters.Payment
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty; // Сообщение об ошибке или описании успешного выполнения
        public string PaymentId { get; set; } = string.Empty; // Уникальный идентификатор транзакции
                                                              // Другие свойства, специфичные для результата платежа
        public string LinkUrl { get; set; } = string.Empty;
        public string? QrLinkUrl { get; set; } = string.Empty;
    }

    public record InitPaymentResult(
        bool Success,
        string ProviderPaymentId,
        string? RedirectUrl,
        string? QrUrl,
        string? ClientSecret);

    public record PaymentInitRequest(
        string ProviderPaymentMethodToken,
        decimal Amount,
        string Currency,
        string OrderId,
        string Description,
        string SuccessReturnUrl);

    public record RefundResult(bool Success, string Message);

    public class PaymentPayload
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Custom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    }

    public record CardMeta(
        string Brand,
        string Last4,
        int ExpMonth,
        int ExpYear);

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded,
        // Другие статусы в зависимости от платежной системы
    }
}
