using System.Text.Json.Serialization;

namespace Flsurf.Domain.User.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRoles
    {
        User, 
        Moderator, 
        Admin, 
        Superadmin 
    }
}
