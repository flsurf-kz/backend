using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Domain.Payment.Exceptions
{
    public class PurchaseIsAlreadyCompleted : Exception
    {
        public PurchaseIsAlreadyCompleted(PurchaseEntity purchase)
            : base($"Purchase is already completed, purchaseid: {purchase.Id}")
        {
        }
    }

    public class PurchaseIsNotConfirmed : Exception
    {
        public PurchaseIsNotConfirmed(PurchaseEntity purchase) 
            : base($"Purchase is not confirmed, confirm by other member of purchase, purchaseId: {purchase.Id}")
        {

        }
    }
}
