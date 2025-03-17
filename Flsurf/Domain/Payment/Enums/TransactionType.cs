using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Deposit,            // Пополнение счета (ввод средств)
        Withdrawal,         // Вывод средств (снятие с баланса)
        Transfer,           // Перевод между кошельками (внутренний перевод)
        Refund,             // Возврат средств (например, при отмене контракта)
        SystemAdjustment,   // Системное изменение баланса (административное действие)
        Bonus,              // Начисление бонусов или промо-акций
        Penalty,            // Штраф (например, за отмену контракта)
        Rollback
    }

}
