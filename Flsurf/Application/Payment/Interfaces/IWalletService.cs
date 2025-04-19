using Flsurf.Application.Payment.Commands;
using Flsurf.Application.Payment.Queries;
using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Interfaces
{
    public interface IWalletService
    {
        BalanceOperation BalanceOperation();
        GetWallet GetWallet();
        BlockWallet BlockWallet();
        AddPaymentMethodHandler AddPaymentMethod();
        RemovePaymentMethodHandler RemovePaymentMethod(); 
        GetPaymentMethodsHandler GetPaymentMethods();
    }
}
