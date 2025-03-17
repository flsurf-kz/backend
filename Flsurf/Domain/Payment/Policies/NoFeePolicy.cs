using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Domain.Payment.Policies
{
    public class NoFeePolicy : IFeePolicy
    {
        public NoFeePolicy() { }

        public Money CalculateFee(Money amount, TransactionType transactionType, FeeContext? context)
        {
            return new Money(0); 
        }
    }
}
