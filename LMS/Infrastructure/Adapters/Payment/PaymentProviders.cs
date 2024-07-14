using System.Text.Json.Serialization;

namespace Flsurf.Infrastructure.Adapters.Payment
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentProviders
    {
        BankCardRu,
        Balance,
        Test
    }
}
