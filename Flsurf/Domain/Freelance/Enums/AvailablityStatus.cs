using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AvailabilityStatus
    {
        Open,      // Открыт к работе
        Busy,      // Занят
        Vacation   // В отпуске
    }

}
