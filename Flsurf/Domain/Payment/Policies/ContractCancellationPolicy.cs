using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Domain.Payment.Policies
{

    /// <summary>
    /// Политика штрафа за отмену контракта (например, процент от суммы),
    /// применяется, когда в контексте отражено, что происходит отмена контракта.
    /// </summary>
    public class ContractCancellationFeePolicy : IFeePolicy
    {
        private readonly decimal _penaltyPercentage;

        public ContractCancellationFeePolicy(decimal penaltyPercentage)
        {
            _penaltyPercentage = penaltyPercentage;
        }

        public Money CalculateFee(Money amount, TransactionType transactionType, FeeContext? context)
        {
            // Здесь предполагаем, что данная политика применяется только при отмене контракта.
            // Вычисляем штраф как процент от суммы.
            var feeAmount = amount.Amount * (_penaltyPercentage / 100);
            return new Money(feeAmount, amount.Currency);
        }
    }
}
