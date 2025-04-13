using System.Text.Json.Serialization;

namespace Flsurf.Application.Common.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortOption
    {
        Date,
        Recomended, 
    }
}
