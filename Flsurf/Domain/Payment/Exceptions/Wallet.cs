using Flsurf.Domain.Payment.Enums;

namespace Flsurf.Domain.Payment.Exceptions
{
    public class WalletIsBlocked : Exception
    {
        public Guid WalletId { get; set; }
        public WalletBlockReason WalletBlockReason { get; set; }

        public WalletIsBlocked(Guid walletId, WalletBlockReason reason) : base("Wallet is blocked")
        {
            WalletId = walletId;
            WalletBlockReason = reason; 
        }
    }

    public class NotEnoughMoneyException : Exception
    {
        public Guid WalletId { get; set; }

        public NotEnoughMoneyException(Guid walletId) : base($"Not enough money, walletId: {walletId}")
        {
            WalletId = walletId;
        }
    }
}
