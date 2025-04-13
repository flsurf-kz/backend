using System.Text.Json.Serialization;

namespace Flsurf.Application.Common.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortByTypes
    {
        Ascending,
        Descending,
        Default, 
        Recommended 
    }
}
