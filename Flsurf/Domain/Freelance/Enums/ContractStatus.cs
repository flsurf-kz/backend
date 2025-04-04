﻿using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContractStatus
    {
        PendingApproval, // Ожидает подтверждения
        Active,          // В процессе выполнения
        Paused,          // Временно приостановлен
        Completed,       // Успешно завершен
        Disputed,        // Арбитраж
        Cancelled,       // Отменен
        Expired,         // Истек срок
        Closed,          // Завершен (больше неактивен)
        PendingFinishApproval,  // Ожидает подтверждения окончания контракта 
    }
}
