using Flsurf.Application.Payment.Commands;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IServiceProvider _serviceProvider;

        public TransactionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public HandleTransaction HandleTransaction()
        {
            return _serviceProvider.GetRequiredService<HandleTransaction>();
        }
        public GetTransactionsList GetTransactionsList()
        {
            return _serviceProvider.GetRequiredService<GetTransactionsList>();
        }
        public GetTransactionProviders GetTransactionProviders()
        {
            return _serviceProvider.GetRequiredService<GetTransactionProviders>();
        }

        public HandleDepositGatewayResult HandleDepositGatewayResult()
        {
            return _serviceProvider.GetRequiredService<HandleDepositGatewayResult>();
        }

        public RefundTransaction RefundTransaction()
        {
            return _serviceProvider.GetRequiredService<RefundTransaction>();
        }

        public HandleWithdrawalGatewayResult HandleWithdrawlGatewayResult()
        {
            return _serviceProvider.GetRequiredService<HandleWithdrawalGatewayResult>();
        }
    }
}
