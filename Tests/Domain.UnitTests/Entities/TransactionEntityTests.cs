using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.Policies;
using Flsurf.Domain.Payment.ValueObjects;
using NUnit.Framework;

namespace Tests.Domain.UnitTests.Entities
{
    [TestFixture]
    public class TransactionEntityTests
    {
        private Guid _walletId;
        private Money _amount;
        private IFeePolicy _feePolicy;

        [SetUp]
        public void SetUp()
        {
            _walletId = Guid.NewGuid();
            _amount = new Money(100, CurrencyEnum.RussianRuble);
            _feePolicy = new NoFeePolicy(); // Для простоты
        }

        [Test]
        public void TransactionEntity_Create_ShouldInitializeProperly()
        {
            // Act
            var tx = TransactionEntity.Create(
                walletId: _walletId,
                amount: _amount,
                feePolicy: _feePolicy,
                type: TransactionType.Deposit,
                flow: TransactionFlow.Incoming,
                comment: "Тестовая транзакция");

            // Assert
            Assert.That(tx, Is.Not.Null);
            Assert.That(tx.WalletId, Is.EqualTo(_walletId));
            Assert.That(tx.RawAmount, Is.EqualTo(_amount));
            Assert.That(tx.NetAmount, Is.EqualTo(_amount)); // NoFeePolicy
            Assert.That(tx.Status, Is.EqualTo(TransactionStatus.Pending));
            Assert.That(tx.Type, Is.EqualTo(TransactionType.Deposit));
            Assert.That(tx.Flow, Is.EqualTo(TransactionFlow.Incoming));
        }

        [Test]
        public void TransactionEntity_Complete_ShouldMarkTransactionAsCompleted()
        {
            // Arrange
            var tx = TransactionEntity.Create(
                _walletId, _amount, _feePolicy,
                TransactionType.Withdrawal,
                TransactionFlow.Outgoing,
                "завершение");

            // Act
            tx.Complete();

            // Assert
            Assert.That(tx.Status, Is.EqualTo(TransactionStatus.Completed));
            Assert.That(tx.CompletedAt, Is.Not.Null);
        }

        [Test]
        public void TransactionEntity_Cancel_ShouldMarkTransactionAsCancelled()
        {
            // Arrange
            var tx = TransactionEntity.Create(
                _walletId, _amount, _feePolicy,
                TransactionType.Refund,
                TransactionFlow.Outgoing,
                "отмена");

            // Act
            tx.Cancel();

            // Assert
            Assert.That(tx.Status, Is.EqualTo(TransactionStatus.Cancelled));
        }

        [Test]
        public void TransactionEntity_ConfirmFromGateway_ShouldComplete_WhenPropsExist()
        {
            // Arrange
            var props = TransactionPropsEntity.CreateGatewayProps(
                paymentUrl: "https://pay.com/start",
                successUrl: "https://site.com/success",
                paymentGateway: "TestGateway",
                feeContext: new FeeContext {  }, "");

            var tx = TransactionEntity.CreateWithProvider(
                _walletId, _amount, TransactionFlow.Incoming,
                TransactionType.Deposit, props, new TransactionProviderEntity(), _feePolicy);

            // Act
            tx.ConfirmFromGateway();

            // Assert
            Assert.That(tx.Status, Is.EqualTo(TransactionStatus.Completed));
            Assert.That(tx.CompletedAt, Is.Not.Null);
        }

        [Test]
        public void TransactionEntity_ConfirmFromGateway_ShouldThrow_WhenPropsNull()
        {
            // Arrange
            var tx = TransactionEntity.Create(
                _walletId, _amount, _feePolicy,
                TransactionType.Deposit,
                TransactionFlow.Incoming,
                null);

            // Act + Assert
            var ex = Assert.Throws<DomainException>(() => tx.ConfirmFromGateway());
            Assert.That(ex!.Message, Is.EqualTo("Не та транзакция для потверждения"));
        }
    }
}
