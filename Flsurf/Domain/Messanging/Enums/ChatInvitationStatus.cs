using System.Text.Json.Serialization;

namespace Flsurf.Domain.Messanging.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChatInvitationStatus
    {
        Rejected, 
        Accepted, 
        Waiting
    }
}
