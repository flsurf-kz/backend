using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Exceptions;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using Flsurf.Domain.User.Entities;
using NUnit.Framework;

namespace Tests.Domain.UnitTests.Entities;

[TestFixture]
public class WalletEntityTests
{
    private WalletEntity _wallet;
    private WalletEntity _receiverWallet;
    private UserEntity _user;

    [SetUp]
    public void Setup()
    {
        _user = new UserEntity(); // допиши, если нужен .Create
        _wallet = WalletEntity.Create(_user);
        _receiverWallet = WalletEntity.Create(new UserEntity());
    }

    [Test]
    public void Create_ShouldInitializeWalletCorrectly()
    {
        Assert.That(_wallet.Currency, Is.EqualTo(CurrencyEnum.RUB));
        Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1000));
        Assert.That(_wallet.Frozen.Amount, Is.EqualTo(0));
        Assert.That(_wallet.Blocked, Is.False);
        Assert.That(_wallet.User, Is.EqualTo(_user));
    }

    [Test]
    public void Deposit_ShouldIncreaseBalance()
    {
        _wallet.BalanceOperation(new Money(300, CurrencyEnum.RUB), BalanceOperationType.Deposit);

        Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1300));
    }

    [Test]
    public void Withdraw_ShouldDecreaseBalance()
    {
        _wallet.BalanceOperation(new Money(400, CurrencyEnum.RUB), BalanceOperationType.Withdrawl);

        Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(600));
    }

    [Test]
    public void Withdraw_InsufficientFunds_ShouldThrow()
    {
        Assert.Throws<NotEnoughMoneyException>(() =>
            _wallet.BalanceOperation(new Money(2000, CurrencyEnum.RUB), BalanceOperationType.Withdrawl));
    }

    [Test]
    public void Freeze_ShouldMoveFundsToFrozen()
    {
        _wallet.BalanceOperation(new Money(300, CurrencyEnum.RUB), BalanceOperationType.Freeze);

        Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(700));
        Assert.That(_wallet.Frozen.Amount, Is.EqualTo(300));
    }

    [Test]
    public void Unfreeze_ShouldMoveFundsToAvailable()
    {
        _wallet.BalanceOperation(new Money(200, CurrencyEnum.RUB), BalanceOperationType.Freeze);
        _wallet.BalanceOperation(new Money(200, CurrencyEnum.RUB), BalanceOperationType.Unfreeze);

        Assert.That(_wallet.Frozen.Amount, Is.EqualTo(0));
        Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1000));
    }

    [Test]
    public void Transfer_ShouldMoveFundsToAnotherWalletWithFreeze()
    {
        _wallet.Transfer(
            new Money(300, CurrencyEnum.RUB),
            _receiverWallet,
            new NoFeePolicy(),
            2
        );

        Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(700));
        Assert.That(_receiverWallet.Frozen.Amount, Is.EqualTo(300));
    }

    [Test]
    public void RefundTransaction_ShouldTransferBack()
    {
        var tx = TransactionEntity.Create(
            walletId: _wallet.Id,
            amount: new Money(150, CurrencyEnum.RUB),
            feePolicy: new NoFeePolicy(),
            type: TransactionType.Transfer,
            flow: TransactionFlow.Outgoing,
            comment: "Тест откат");

        _wallet.AcceptTransaction(tx);
        _wallet.RefundTransaction(tx, _receiverWallet);

        Assert.That(_receiverWallet.AvailableBalance.Amount, Is.GreaterThan(1000));
    }

    [Test]
    public void Block_ShouldPreventAllOperations()
    {
        _wallet.Block(WalletBlockReason.LegalIssue);

        Assert.Throws<WalletIsBlocked>(() =>
            _wallet.BalanceOperation(new Money(100, CurrencyEnum.RUB), BalanceOperationType.Deposit));

        Assert.That(_wallet.Blocked, Is.True);
        Assert.That(_wallet.BlockReason, Is.EqualTo(WalletBlockReason.LegalIssue));
    }

    [Test]
    public void RefundTransactionWithoutReceiver_ShouldCreateTransaction()
    {
        var tx = TransactionEntity.Create(
            walletId: _wallet.Id,
            amount: new Money(200, CurrencyEnum.RUB),
            feePolicy: new NoFeePolicy(),
            type: TransactionType.Deposit,
            flow: TransactionFlow.Incoming,
            comment: "for test");

        _wallet.AcceptTransaction(tx);

        Assert.DoesNotThrow(() =>
            _wallet.RefundTransactionWithoutReceiver(tx, new NoFeePolicy()));
    }

    [Test]
    public void VerifyTransaction_ShouldReturnTrue_WhenValid()
    {
        var tx = TransactionEntity.Create(
            walletId: _wallet.Id,
            amount: new Money(100, CurrencyEnum.RUB),
            feePolicy: new NoFeePolicy(),
            type: TransactionType.Deposit,
            flow: TransactionFlow.Incoming,
            comment: "Проверка");

        Assert.That(_wallet.VerifyTransaction(tx), Is.True);
    }
}
