using Flsurf.Application.Files.Permissions;
using Flsurf.Domain.Payment.Entities;
using SpiceDb.Models;

namespace Flsurf.Application.Payment.Permissions
{
    public class ZedPaymentUser : ResourceReference
    {
        private ZedPaymentUser(Guid userId) : base($"flsurf/file:{userId}") { }

        public static ZedPaymentUser WithId(Guid userId) => new(userId);

        public Permission CanAddBalance(ZedWallet wallet) => new(this, "add_balance", wallet);
        
        public Permission CanBlockWallet(ZedWallet wallet) => new(this, "block", wallet);

        public Permission CanReadWallet(ZedWallet wallet) => new(this, "read", wallet); 
    }
}
