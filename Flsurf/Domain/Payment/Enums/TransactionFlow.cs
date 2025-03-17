using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionFlow
    {
        [Display(Name = "IN")]
        Incoming,
        [Display(Name = "OUT")]
        Outgoing,
        [Display(Name = "INTERNAL")]
        Internal, 
    }
}
