using SpiceDb.Models;

namespace Flsurf.Application.Payment.Permissions
{
    public class ZedTransaction : ResourceReference
    {
        private ZedTransaction(string transactionId)
            : base($"flsurf/transaction:{transactionId}") { }

        public static ZedTransaction WithId(Guid transactionId) =>
            new ZedTransaction(transactionId.ToString());

        public static ZedTransaction WithWildcard() =>
            new ZedTransaction("*");
    }
}
