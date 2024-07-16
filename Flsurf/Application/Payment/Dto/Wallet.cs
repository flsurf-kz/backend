using Flsurf.Domain.Payment.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.Dto
{
    public class BalanceOperationDto
    {
        [Required]
        public Guid WalletId { get; set; }
        [Required]
        public Money Balance { get; set; } = new(0);
    }

    public class GetWalletDto
    {
        public Guid? WalletId { get; set; }
        public Guid? UserId { get; set; }
    }

    public class BlockWalletDto
    {
        [Required]
        public Guid WalletId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
