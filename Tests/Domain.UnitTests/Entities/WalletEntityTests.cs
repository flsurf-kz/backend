
namespace Tests.Domain.UnitTests.Entities
{
    using Flsurf.Domain.Payment.Entities;
    using Flsurf.Domain.Payment.Enums;
    using Flsurf.Domain.Payment.Exceptions;
    using Flsurf.Domain.Payment.ValueObjects;
    using Flsurf.Domain.User.Entities;
    using NUnit.Framework;
    using System;

    namespace WalletTests
    {
        [TestFixture]
        public class WalletEntityTests
        {
            private WalletEntity _wallet;
            private WalletEntity _receiverWallet;
            private UserEntity _user;

            [SetUp]
            public void SetUp()
            {
                _user = new UserEntity();
                _wallet = WalletEntity.Create(_user);
                _receiverWallet = WalletEntity.Create(new UserEntity());
            }

            #region ✅ Создание кошелька

            [Test]
            public void WalletEntity_Create_Should_Set_Default_Values_Correctly()
            {
                Assert.That(_wallet, Is.Not.Null);
                Assert.That(_wallet.User, Is.EqualTo(_user));
                Assert.That(_wallet.Currency, Is.EqualTo(CurrencyEnum.RussianRuble));
                Assert.That(_wallet.Frozen.Amount, Is.EqualTo(0));
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1000));
                Assert.That(_wallet.PendingIncome.Amount, Is.EqualTo(0));
                Assert.That(_wallet.Blocked, Is.False);
            }

            #endregion

            #region ✅ Пополнение средств

            [Test]
            public void Deposit_ShouldIncreaseAvailableBalance()
            {
                // Act
                _wallet.Deposit(new Money(500, CurrencyEnum.RussianRuble));

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1500));
            }

            [Test]
            public void Deposit_WhenWalletBlocked_ShouldThrowException()
            {
                // Arrange
                _wallet.Block(WalletBlockReason.FraudSuspicion);

                // Act & Assert
                Assert.Throws<WalletIsBlocked>(() =>
                    _wallet.Deposit(new Money(500, CurrencyEnum.RussianRuble)));
            }

            #endregion

            #region ✅ Списание средств

            [Test]
            public void Withdraw_ShouldDecreaseAvailableBalance()
            {
                // Act
                _wallet.Withdraw(new Money(300, CurrencyEnum.RussianRuble));

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(700));
            }

            [Test]
            public void Withdraw_WhenInsufficientFunds_ShouldThrowException()
            {
                // Act & Assert
                Assert.Throws<NotEnoughMoneyException>(() =>
                    _wallet.Withdraw(new Money(2000, CurrencyEnum.RussianRuble)));
            }

            [Test]
            public void Withdraw_WhenWalletBlocked_ShouldThrowException()
            {
                // Arrange
                _wallet.Block(WalletBlockReason.LegalIssue);

                // Act & Assert
                Assert.Throws<WalletIsBlocked>(() =>
                    _wallet.Withdraw(new Money(200, CurrencyEnum.RussianRuble)));
            }

            #endregion

            #region 🔄 Перевод средств

            [Test]
            public void Transfer_ShouldMoveFundsBetweenWallets()
            {
                // Act
                _wallet.TransferTo(_receiverWallet, new Money(300, CurrencyEnum.RussianRuble));

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(700));
                Assert.That(_receiverWallet.AvailableBalance.Amount, Is.EqualTo(1300));
            }

            [Test]
            public void Transfer_WhenSenderBlocked_ShouldThrowException()
            {
                // Arrange
                _wallet.Block(WalletBlockReason.LegalIssue);

                // Act & Assert
                Assert.Throws<WalletIsBlocked>(() =>
                    _wallet.TransferTo(_receiverWallet, new Money(300, CurrencyEnum.RussianRuble)));
            }

            [Test]
            public void Transfer_WhenInsufficientFunds_ShouldThrowException()
            {
                // Arrange
                _wallet.Withdraw(new Money(900, CurrencyEnum.RussianRuble)); // Остаток 100 RUB

                // Act & Assert
                Assert.Throws<NotEnoughMoneyException>(() =>
                    _wallet.TransferTo(_receiverWallet, new Money(300, CurrencyEnum.RussianRuble)));
            }

            #endregion

            #region 🔒 Заморозка и разморозка средств

            [Test]
            public void FreezeAmount_ShouldReduceAvailableBalanceAndIncreaseFrozen()
            {
                // Act
                _wallet.FreezeAmount(new Money(200, CurrencyEnum.RussianRuble));

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(800));
                Assert.That(_wallet.Frozen.Amount, Is.EqualTo(200));
            }

            [Test]
            public void UnfreezeAmount_ShouldReduceFrozenAndIncreaseAvailableBalance()
            {
                // Arrange
                _wallet.FreezeAmount(new Money(200, CurrencyEnum.RussianRuble));

                // Act
                _wallet.UnfreezeAmount(new Money(200, CurrencyEnum.RussianRuble));

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1000));
                Assert.That(_wallet.Frozen.Amount, Is.EqualTo(0));
            }

            [Test]
            public void FreezeAmount_WhenInsufficientFunds_ShouldThrowException()
            {
                // Act & Assert
                Assert.Throws<NotEnoughMoneyException>(() =>
                    _wallet.FreezeAmount(new Money(2000, CurrencyEnum.RussianRuble)));
            }

            #endregion

            #region ❌ Откат транзакции

            [Test]
            public void RollbackTransaction_ShouldRestoreBalanceForOutgoingTransaction()
            {
                // Arrange
                var transaction = new TransactionEntity(
                    _wallet.Id,
                    new Money(300, CurrencyEnum.RussianRuble),
                    TransactionFlow.Outgoing
                );

                // Act
                _wallet.RollbackTransaction(transaction);

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(1300));
            }

            [Test]
            public void RollbackTransaction_ShouldReduceBalanceForIncomingTransaction()
            {
                // Arrange
                var transaction = new TransactionEntity(
                    _wallet.Id,
                    new Money(200, CurrencyEnum.RussianRuble),
                    TransactionFlow.Incoming
                );

                // Act
                _wallet.RollbackTransaction(transaction);

                // Assert
                Assert.That(_wallet.AvailableBalance.Amount, Is.EqualTo(800));
            }

            #endregion

            #region 🚨 Блокировка кошелька

            [Test]
            public void Block_ShouldSetBlockedStatusAndReason()
            {
                // Act
                _wallet.Block(WalletBlockReason.FraudSuspicion);

                // Assert
                Assert.That(_wallet.Blocked, Is.True);
                Assert.That(_wallet.BlockReason, Is.EqualTo(WalletBlockReason.FraudSuspicion));
            }

            [Test]
            public void Block_WhenWalletAlreadyBlocked_ShouldThrowException()
            {
                // Arrange
                _wallet.Block(WalletBlockReason.FraudSuspicion);

                // Act & Assert
                Assert.Throws<WalletIsBlocked>(() =>
                    _wallet.Block(WalletBlockReason.LegalIssue));
            }

            #endregion
        }
    }

}
