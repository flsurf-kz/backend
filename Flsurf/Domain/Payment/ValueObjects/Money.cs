﻿using Flsurf.Domain.Common;
using Flsurf.Domain.Payment.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flsurf.Domain.Payment.ValueObjects
{
    [Owned]
    public class Money : ValueObject
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public CurrencyEnum Currency { get; set; }

        public Money(decimal amount, CurrencyEnum currency = CurrencyEnum.RussianRuble)
        {
            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public static void VerifyCurrency(Money lhs, Money rhs)
        {
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                throw new ArgumentNullException("One or both Money objects are null");
            }

            if (lhs.Currency != rhs.Currency)
            {
                throw new ArgumentException("Currencies are not equal");
            }
        }

        public static bool operator >(Money lhs, Money rhs)
        {
            VerifyCurrency(lhs, rhs);
            return lhs.Amount > rhs.Amount;
        }

        public static bool operator <(Money lhs, Money rhs)
        {
            VerifyCurrency(lhs, rhs);
            return lhs.Amount < rhs.Amount;
        }

        public static bool operator ==(Money lhs, Money rhs)
        {
            VerifyCurrency(lhs, rhs);
            return lhs.Amount == rhs.Amount;
        }

        public static bool operator !=(Money lhs, Money rhs)
        {
            VerifyCurrency(lhs, rhs);
            return lhs.Amount != rhs.Amount;
        }

        public static Money operator -(Money lhs, Money rhs)
        {
            VerifyCurrency(lhs, rhs);
            return new Money(lhs.Amount - rhs.Amount, lhs.Currency);
        }

        public static Money operator +(Money lhs, Money rhs)
        {
            VerifyCurrency(lhs, rhs);
            return new Money(lhs.Amount + rhs.Amount, lhs.Currency);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Money other = (Money)obj;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }
    }
}
