using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WorkSnapshotStatus
    {
        Pause, 
        SessionCompleted, 
        SessionStarted, 
        Resume, 
    }
}
