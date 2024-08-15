using SpiceDb.Models;

namespace Flsurf.Application.Payment.Permissions
{
    public class ZedPurchase : ResourceReference
    {
        private ZedPurchase(string purchaseId) : base($"flsurf/purchase:{purchaseId}") { }

        public static ZedPurchase WithId(Guid purchaseId) => new ZedPurchase(purchaseId.ToString());

        public static ZedPurchase WithWildcard() => new ZedPurchase("*");
    }
}
