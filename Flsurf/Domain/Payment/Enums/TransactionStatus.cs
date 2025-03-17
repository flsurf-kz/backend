using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionStatus
    {
        Pending,             // Ожидание подтверждения
        Processing,          // В процессе обработки (длительная операция)
        Completed,           // Успешно завершена
        Failed,              // Неуспешная попытка (например, отказ со стороны банка)
        Cancelled,           // Отменена (пользователем или системой)
        Expired,             // Истек срок ожидания транзакции
        Reversed             // Сторнирована (деньги возвращены после завершения)
    }
}
