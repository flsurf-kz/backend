using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JobLevel
    {
        Beginner, 
        Intermediate, 
        Expert, 
    }
}
