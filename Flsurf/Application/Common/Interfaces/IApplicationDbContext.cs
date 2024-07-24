using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;

namespace Flsurf.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        // User 
        DbSet<UserEntity> Users { get; set; }
        DbSet<WarningEntity> UserWarnings { get; set; }
        DbSet<NotificationEntity> Notifications { get; set; }

        // payment 
        DbSet<TransactionProviderEntity> TransactionProviders { get; set; }
        DbSet<PaymentSystemEntity> PaymentSystems { get; set; }
        DbSet<PurchaseEntity> Purchases { get; set; }
        DbSet<TransactionEntity> Transactions { get; set; }
        DbSet<WalletEntity> Wallets { get; set; }

        // Staff 
        DbSet<TicketSubjectEntity> TicketSubjects { get; set; }
        DbSet<TicketEntity> Tickets { get; set; }
        DbSet<TicketCommentEntity> TicketComments { get; set; }

        // Files 
        DbSet<FileEntity> Files { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);
    }
}
