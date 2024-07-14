using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PurchaseStatus
    {
        Teaching,   
        Processing,
        Failed,
        Success,
        Rejected,
        Expired,
    }
}
