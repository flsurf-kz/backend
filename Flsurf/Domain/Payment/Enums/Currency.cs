using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrencyEnum
    {
        [Display(Name = "RUB")]
        RUB,
        [Display(Name = "USD")]
        USD,
        [Display(Name = "KZT")]
        KZT,
        [Display(Name = "EUR")]
        EUR
    }
}
