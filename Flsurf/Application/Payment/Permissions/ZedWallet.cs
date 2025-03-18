using SpiceDb.Models;

namespace Flsurf.Application.Payment.Permissions
{
    public class ZedWallet : ResourceReference
    {
        private ZedWallet(Guid walletId) : base($"flsurf/file:{walletId}") { }

        public static ZedWallet WithId(Guid walletId) => new(walletId);

        public Relationship Owner(ZedPaymentUser user) => new(user, "owner", this); 
    }
}
