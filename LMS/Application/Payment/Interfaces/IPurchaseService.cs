using Flsurf.Application.Payment.UseCases;

namespace Flsurf.Application.Payment.Interfaces
{
    public interface IPurchaseService
    {
        CreatePurchase CreatePurchase();
        ConfirmPurchase ConfirmPurchase();
        GetPurchasesList GetPurchasesList();
        UpdatePurchase UpdatePurchase();
        SolveProblem SolveProblem();
    }
}
