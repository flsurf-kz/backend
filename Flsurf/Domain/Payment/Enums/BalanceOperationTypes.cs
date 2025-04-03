using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    // admin only!! 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BalanceOperationType
    {
        Freeze,
        Unfreeze,
        PendingIncome, 
        Deposit, 
        Withdrawl
    }
}
