using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PurchaseStatus
    {
        Pending,      // Ожидание завершения
        Completed,    // Завершена успешно
        Failed,       // Завершена с ошибкой
        Canceled      // Отменена
    }
}
