using Flsurf.Domain.Payment.Enums;
using Flsurf.Domain.Payment.ValueObjects;

namespace Flsurf.Domain.Payment.Policies
{
    public interface IFeePolicy
    {
        Money CalculateFee(Money amount, TransactionType transactionType, FeeContext? context);
    }
}
