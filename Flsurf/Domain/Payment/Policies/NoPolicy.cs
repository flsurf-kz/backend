using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Domain.Payment.Policies
{
    public class NoPolicy : IFeePolicy
    {
        public NoPolicy() { }

        public Money CalculateFee(Money amount, TransactionType transactionType, FeeContext? context)
        {
            return new Money(0); 
        }
    }
}
