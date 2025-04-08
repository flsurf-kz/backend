using System.Text.Json.Serialization;

namespace Flsurf.Domain.User.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserTypes
    {
        Freelancer, 
        Client, 
        Other, 
        NonUser, 
        Staff, 
    }
}
