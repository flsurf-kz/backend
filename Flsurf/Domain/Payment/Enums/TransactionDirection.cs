using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionDirection
    {
        [Display(Name = "IN")]
        In,
        [Display(Name = "OUT")]
        Out,
    }
}
