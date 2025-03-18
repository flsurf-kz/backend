using Flsurf.Application.Common.cqrs;
using Flsurf.Domain.Payment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.Commands
{
    public class GatewayResultCommand : BaseCommand
    {
        [Required]
        public bool Success { get; set; }

        [Required]
        public string GatewayTransactionId { get; set; } = string.Empty; // ID транзакции на стороне шлюза (например, Stripe)

        [Required]
        public string InternalTransactionId { get; set; } = string.Empty; // твой внутренний ID транзакции

        public string CustomId { get; set; } = string.Empty; // твой дополнительный ID (если нужен)

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal Fee { get; set; }

        [Required]
        public CurrencyEnum Currency { get; set; }

        public string Status { get; set; } = string.Empty; // дополнительный статус, например из Stripe ("paid", "failed", "pending")

        public string FailureReason { get; set; } = string.Empty; // причина ошибки от шлюза (если есть)

        public Dictionary<string, string>? Metadata { get; set; } // для любых доп. данных от шлюза
    }

}
