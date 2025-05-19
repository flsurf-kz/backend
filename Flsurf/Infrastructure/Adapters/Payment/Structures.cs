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

    public record PrepareCardSetupRequest
    {
        public Guid UserId { get; init; }
        public string? CustomerIdInProvider { get; init; } // ID клиента на стороне платежного провайдера (Stripe Customer ID)
        public string ReturnUrl { get; init; } // URL, куда пользователь вернется после завершения на стороне провайдера (если применимо) или после виджета
        public Dictionary<string, string>? Metadata { get; init; }
    }

    public record CardSetupDetails
    {
        public bool Success { get; init; }
        public string ProviderSetupId { get; init; } = string.Empty; // ID операции настройки у провайдера (например, Stripe SetupIntent ID - "seti_...")
        public string? ClientSecretForWidget { get; init; } // Для инициализации виджета на фронтенде (Stripe Elements)
        public string? RedirectUrlForProviderPage { get; init; } // Если настройка происходит на странице провайдера
        public string? ErrorMessage { get; init; }
    }

    /// <summary>
    /// Запрос на инициацию платежа.
    /// </summary>
    public record PaymentRequestDetails // Похож на ваш PaymentInitRequest
    {
        public Guid UserId { get; init; }
        public string? CustomerIdInProvider { get; init; } // ID клиента на стороне провайдера
        public decimal Amount { get; init; }
        public string Currency { get; init; } // ISO 4217 код
        public string InternalTransactionId { get; init; } // ID вашей внутренней транзакции/заказа
        public string Description { get; init; }
        public string SuccessReturnUrl { get; init; } // URL для возврата после успешной оплаты
        public string CancelReturnUrl { get; init; }  // URL для возврата при отмене
        public string? PaymentMethodToken { get; init; } // Токен сохраненного способа оплаты (pm_... для Stripe)
        public bool SetupFutureUsage { get; init; } = false; // Пытаться ли сохранить способ оплаты для будущих использований (если новый)
        public Dictionary<string, string>? Metadata { get; init; }
    }

    /// <summary>
    /// Результат инициации платежа.
    /// Содержит данные, необходимые фронтенду для отображения виджета или редиректа.
    /// Это похоже на ваш InitPaymentResult, но более явно разделено.
    /// </summary>
    public record PaymentExecutionDetails
    {
        public bool Success { get; init; }
        public string ProviderPaymentId { get; init; } = string.Empty; // ID платежа у провайдера (Stripe PaymentIntent ID - "pi_...")
        public string? ClientSecretForWidget { get; init; } // Для инициализации виджета на фронтенде (Stripe PaymentElement)
        public string? RedirectUrlForProviderPage { get; init; } // Если оплата происходит на странице провайдера
        public string? QrUrl { get; init; } // Опционально, для QR-платежей
        public string? ErrorMessage { get; init; }
    }
}
