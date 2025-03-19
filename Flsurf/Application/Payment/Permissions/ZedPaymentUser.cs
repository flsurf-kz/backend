using SpiceDb.Models;

namespace Flsurf.Application.Payment.Permissions
{
    public class ZedPaymentUser : ResourceReference
    {
        private ZedPaymentUser(Guid userId) : base($"flsurf/file:{userId}") { }

        public static ZedPaymentUser WithId(Guid userId) => new(userId);

        public Permission CanAddBalance(ZedWallet wallet) => new(wallet, "add_balance", this);
        
        public Permission CanBlockWallet(ZedWallet wallet) => new(wallet, "block", this);

        public Permission CanReadWallet(ZedWallet wallet) => new(wallet, "read", this);

        public Permission CanReadPurchases() => new(this, "read", ZedPurchase.WithWildcard());
        public Permission CanRefundTransaction(ZedTransaction tx) => new(tx, "refund", this); 
    }
}
