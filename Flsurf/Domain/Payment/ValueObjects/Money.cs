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

        private Money() { }

        public Money(decimal amount, CurrencyEnum currency = CurrencyEnum.RUB)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            Amount = Math.Round(amount, 2); // Округляем до копеек (2 знака после запятой)
            Currency = currency;
        }

        private Money(decimal amount, CurrencyEnum currency, bool bypassValidation)
        {

            Amount = amount;
            Currency = currency;
        }

        public static Money Null() => new Money(0, CurrencyEnum.RUB, true);

        // huinay
        public Money(Money money)
        {
            if (money.Amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            Amount = money.Amount * 1; 
            Currency = money.Currency;
        }

        // ✅ Проверка валюты (упрощенный вариант)
        private static void EnsureSameCurrency(Money lhs, Money rhs)
        {
            if (lhs.Currency != rhs.Currency)
                throw new ArgumentException("Currencies are not equal");
        }

        // ✅ Операторы сравнения
        public static bool operator >(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount > rhs.Amount;
        }

        public static bool operator <(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount < rhs.Amount;
        }

        public static bool operator >=(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount >= rhs.Amount;
        }

        public static bool operator <=(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount <= rhs.Amount;
        }

        public static bool operator ==(Money lhs, Money rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;
            if (lhs is null || rhs is null)
                return false;

            EnsureSameCurrency(lhs, rhs);
            return lhs.Amount == rhs.Amount;
        }

        public static bool operator !=(Money lhs, Money rhs)
        {
            return !(lhs == rhs);
        }

        // ✅ Операторы сложения и вычитания
        public static Money operator +(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            return new Money(lhs.Amount + rhs.Amount, lhs.Currency);
        }

        public static Money operator -(Money lhs, Money rhs)
        {
            EnsureSameCurrency(lhs, rhs);
            if (lhs.Amount < rhs.Amount)
                throw new InvalidOperationException("Resulting amount cannot be negative.");

            return new Money(lhs.Amount - rhs.Amount, lhs.Currency);
        }

        public static Money operator -(Money lhs, decimal rhs)
        {
            if (lhs.Amount < rhs)
                throw new InvalidOperationException("Resulting amount cannot be negative.");

            return new Money(lhs.Amount - rhs, lhs.Currency);
        }


        // ✅ Умножение и деление (для работы с комиссиями или конвертацией)
        public static Money operator *(Money lhs, decimal multiplier)
        {
            if (multiplier < 0)
                throw new ArgumentException("Multiplier cannot be negative.");

            return new Money(lhs.Amount * multiplier, lhs.Currency);
        }

        public static Money operator *(Money lhs, int multiplier)
        {
            if (multiplier < 0)
                throw new ArgumentException("Multiplier cannot be negative.");

            return new Money(lhs.Amount * multiplier, lhs.Currency);
        }

        public static Money operator /(Money lhs, decimal divisor)
        {
            if (divisor <= 0)
                throw new ArgumentException("Divisor must be greater than zero.");

            return new Money(lhs.Amount / divisor, lhs.Currency);
        }

        public static Money operator *(Money lhs, double multiplier)
        {
            if (multiplier <= 0)
                throw new ArgumentException("Mulitplier must be greater than zero.");

            return new Money(lhs.Amount * (decimal)multiplier, lhs.Currency);
        }

        public static Money operator /(Money lhs, double divisor)
        {
            if (divisor <= 0)
                throw new ArgumentException("Divisor must be greater than zero.");

            return new Money(lhs.Amount / (decimal)divisor, lhs.Currency);
        }

        // ✅ Методы для проверки нуля
        public bool IsZero() => Amount == 0;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Money other = (Money)obj;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

}
