using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Interfaces
{
    public interface IWalletService
    {
        BalanceOperation BalanceOperation();
        GetWallet GetWallet();
        BlockWallet BlockWallet();
    }
}
