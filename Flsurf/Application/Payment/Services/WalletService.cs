using Flsurf.Application.Payment.Commands;
using Flsurf.Application.Payment.Interfaces;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Flsurf.Application.Payment.Services
{
    public class WalletService : IWalletService
    {
        private readonly IServiceProvider _serviceProvider;

        public WalletService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BalanceOperation BalanceOperation()
        {
            return _serviceProvider.GetRequiredService<BalanceOperation>();
        }
        public GetWallet GetWallet()
        {
            return _serviceProvider.GetRequiredService<GetWallet>();
        }
        public BlockWallet BlockWallet()
        {
            return _serviceProvider.GetRequiredService<BlockWallet>();
        }

        public AddPaymentMethodHandler AddPaymentMethod()
        {
            return _serviceProvider.GetRequiredService<AddPaymentMethodHandler>();
        }

        public RemovePaymentMethodHandler RemovePaymentMethod()
        {
            return _serviceProvider.GetRequiredService<RemovePaymentMethodHandler>();
        }

        public GetPaymentMethodsHandler GetPaymentMethods()
        {
            return _serviceProvider.GetRequiredService<GetPaymentMethodsHandler>();
        }
    }
}
