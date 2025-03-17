using Flsurf.Domain.Payment.Policies;
using Flsurf.Infrastructure;
using Microsoft.Extensions.Options;

namespace Flsurf.Application.Payment.InnerServices
{
    public class FeePolicyService
    {
        private readonly FeeSettings _settings;

        public FeePolicyService(IOptions<FeeSettings> settings)
        {
            _settings = settings.Value;
        }

        public IFeePolicy GetPolicy(TransactionType transactionType, FeeContext context)
        {
            return transactionType switch
            {
                TransactionType.Deposit => new StandardFeePolicy(_settings.DepositFixedFee, _settings.DepositPercentageFee),
                TransactionType.Withdrawal => new StandardFeePolicy(_settings.WithdrawalFixedFee, _settings.WithdrawalPercentageFee),
                TransactionType.Transfer => new NoFeePolicy(),
                TransactionType.Refund when context.IsContractCancellation =>
                    new ContractCancellationFeePolicy(_settings.ContractCancellationFee),

                _ => throw new NotImplementedException("Unknown transaction type")
            };
        }
    }
}
