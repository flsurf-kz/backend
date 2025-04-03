using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContestEntryReaction
    {
        None,
        Like,
        Dislike,
        Winner,
        Insightful
    }
}
