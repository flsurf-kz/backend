using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Domain.Payment.Policies
{
    public class StandardFeePolicy : IFeePolicy
    {
        private readonly decimal _fixedFee;
        private readonly decimal _percentageFee;

        public StandardFeePolicy(decimal fixedFee, decimal percentageFee)
        {
            _fixedFee = fixedFee;
            _percentageFee = percentageFee;
        }

        public Money CalculateFee(Money amount, TransactionType transactionType, FeeContext? context)
        {
            // Вычисляем процент от суммы
            var percentageFeeAmount = amount.Amount * (_percentageFee / 100);
            var feeAmount = _fixedFee + percentageFeeAmount;
            return new Money(feeAmount, amount.Currency);
        }
    }
}
