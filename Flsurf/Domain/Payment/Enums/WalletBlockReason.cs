using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WalletBlockReason
    {
        None,
        FraudSuspicion,
        LegalIssue,
        UserRequest
    }
}
