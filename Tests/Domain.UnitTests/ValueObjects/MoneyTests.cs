using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;
using NUnit.Framework;
using System;

namespace Tests.Domain.UnitTests.ValueObjects;

[TestFixture]
public class MoneyTests
{
    private Money rub100;
    private Money rub50;
    private Money usd100;

    [SetUp]
    public void Setup()
    {
        rub100 = new Money(100, CurrencyEnum.RussianRuble);
        rub50 = new Money(50, CurrencyEnum.RussianRuble);
        usd100 = new Money(100, CurrencyEnum.Dollar);
    }

    [Test]
    public void Constructor_ShouldSetProperties()
    {
        var money = new Money(123.456m, CurrencyEnum.RussianRuble);

        Assert.That(money.Amount, Is.EqualTo(123.46m));
        Assert.That(money.Currency, Is.EqualTo(CurrencyEnum.RussianRuble));
    }

    [Test]
    public void Constructor_NegativeAmount_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Money(-1, CurrencyEnum.Dollar));
    }

    [Test]
    public void Equality_ShouldCompareByValue()
    {
        var a = new Money(100, CurrencyEnum.RussianRuble);
        var b = new Money(100, CurrencyEnum.RussianRuble);

        Assert.That(a == b, Is.True);
        Assert.That(a.Equals(b), Is.True);
    }

    [Test]
    public void Null_ShouldReturnSentinelInstance()
    {
        var money = Money.Null();

        Assert.That(money.Amount, Is.LessThan(0));
    }

    [Test]
    public void Addition_ShouldSumAmounts()
    {
        var result = rub50 + rub100;

        Assert.That(result.Amount, Is.EqualTo(150));
    }

    [Test]
    public void Subtraction_ShouldSubtractAmounts()
    {
        var result = rub100 - rub50;

        Assert.That(result.Amount, Is.EqualTo(50));
    }

    [Test]
    public void Subtraction_WhenNegative_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var result = rub50 - rub100;
        });
    }

    [Test]
    public void ComparisonOperators_ShouldWorkCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(rub100 > rub50, Is.True);
            Assert.That(rub50 < rub100, Is.True);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.That(rub100 >= rub100, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.That(rub50 <= rub100, Is.True);
        });
    }

    [Test]
    public void CurrencyMismatch_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var result = rub100 + usd100;
        });
    }

    [Test]
    public void Multiply_Decimal_ShouldWork()
    {
        var result = rub100 * 1.5m;

        Assert.That(result.Amount, Is.EqualTo(150));
    }

    [Test]
    public void Multiply_Double_ShouldWork()
    {
        var result = rub50 * 2.0;

        Assert.That(result.Amount, Is.EqualTo(100));
    }

    [Test]
    public void Divide_ShouldReturnCorrectAmount()
    {
        var result = rub100 / 2.0;

        Assert.That(result.Amount, Is.EqualTo(50));
    }

    [Test]
    public void Divide_ByZero_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var result = rub100 / 0.0;
        });
    }

    [Test]
    public void IsZero_ShouldReturnTrue_WhenZero()
    {
        var zero = new Money(0, CurrencyEnum.Dollar);

        Assert.That(zero.IsZero(), Is.True);
    }

    [Test]
    public void IsZero_ShouldReturnFalse_WhenNonZero()
    {
        Assert.That(rub100.IsZero(), Is.False);
    }

    [Test]
    public void CopyConstructor_ShouldCopyCorrectly()
    {
        var copy = new Money(rub50);

        Assert.That(copy.Amount, Is.EqualTo(rub50.Amount));
        Assert.That(copy.Currency, Is.EqualTo(rub50.Currency));
    }

    [Test]
    public void Equals_And_HashCode_ShouldMatch()
    {
        var a = new Money(123, CurrencyEnum.RussianRuble);
        var b = new Money(123, CurrencyEnum.RussianRuble);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }
}
