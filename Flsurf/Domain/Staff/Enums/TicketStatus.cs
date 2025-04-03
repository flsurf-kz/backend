using System.Text.Json.Serialization;

namespace Flsurf.Domain.Staff.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TicketStatus
    {
        Open,
        Closed,
        InProgress
    }
}
