using Flsurf.Application.Common.Models;
using Flsurf.Domain.Payment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.Dto
{
    public class ConfirmPurchaseDto
    {
        [Required]
        public Guid PurchaseId { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Rate had to be 1-5")]
        public int Rate { get; set; }
        [Required]
        public string RateText { get; set; } = string.Empty;
    }

    public class CreatePurchaseDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public string ProviderName { get; set; } = string.Empty;
    }

    public class UpdatePurchaseDto
    {

    }

    public class GetPurchasesListDto : InputPagination
    {
        public PurchaseStatus? Status { get; set; }
        public TransactionDirection? Direction { get; set; }
        public TransactionOperations? Operation { get; set; }
        public TransactionStatusEnum? TransactionSatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SoldByUserId { get; set; }
    }
}
