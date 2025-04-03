using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DisputeStatus
    {
        Pending,       // Ожидает принятия модератором
        InReview,      // Находится в процессе рассмотрения
        Resolved       // Завершён
    }
}

