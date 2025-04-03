using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentScheduleType
    {
        Milestone,    // Поэтапно
        Weekly,        // Еженедельно
        Monthly,       // Ежемесячно
        OnCompletion   // По завершению
    }

}
