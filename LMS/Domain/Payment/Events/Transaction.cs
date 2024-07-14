using Flsurf.Domain.Payment.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Payment.Events
{
    public class TransactionCreated(TransactionEntity transaction) : BaseEvent
    {
        public TransactionEntity Transaction { get; set; } = transaction;
    }

    public class TransactionConfirmed(TransactionEntity transaction) : BaseEvent
    {
        public TransactionEntity Transaction { get; set; } = transaction;
    }
}
