﻿using Flsurf.Application.Common.Models;
using Flsurf.Domain.Payment.Enums;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Application.Payment.Dto
{
    public class SolveProblemDto
    {
    }

    public class CreateTransactionDto
    {

    }


    public class GetTransactionsListFilterDto : InputPagination
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TransactionOperations? Operation { get; set; }
        public string? TransactionProvider { get; set; }
    }

    public class GetTransactionsListDto : GetTransactionsListFilterDto
    {
        [Required]
        public Guid WalletId { get; set; }
    }


    public class UpdateTransactionDto
    {

    }

    public class GetTransactionProvidersDto
    {

    }

    public class HandleTransactionDto
    {
        [Required]
        public Guid TransactionId { get; set; }
    }

    public class GatewayResultDto
    {
        [Required]
        public bool Success { get; set; } = false;
        [Required]
        public string CustomId { get; set; } = string.Empty;
        [Required]
        public decimal Fee { get; set; }
        [Required]
        public CurrencyEnum CurrencyIn { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string TransactionId { get; set; } = string.Empty;
        [Required]
        public string Custom { get; set; } = string.Empty;
    }
}
