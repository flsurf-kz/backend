﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flsurf.Domain.Payment.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionOperations
    {
        [Display(Name = "SELL")]
        Sell,

        [Display(Name = "WITHDRAW")]
        Withdraw,

        [Display(Name = "MANUAL_BALANCE_DECREASE")]
        ManualBalanceDecrease,

        [Display(Name = "PRODUCT_PREMIUM_PRIORITY")]
        ProductPremiumPriority,

        [Display(Name = "MANUAL_BALANCE_INCREASE")]
        ManualBalanceIncrease,

        [Display(Name = "DEPOSIT")]
        Deposit,

        [Display(Name = "FROZEN")]
        Frozen,

        [Display(Name = "BUY")]
        Buy,

        [Display(Name = "REFERRAL_BONUS")]
        ReferralBonus,
    }

}
