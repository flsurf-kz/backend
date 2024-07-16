using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        // User 
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<WarningEntity> UserWarnings { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }

        // payment 
        public DbSet<TransactionProviderEntity> TransactionProviders { get; set; }
        public DbSet<PaymentSystemEntity> PaymentSystems { get; set; }
        public DbSet<PurchaseEntity> Purchases { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<WalletEntity> Wallets { get; set; }

        // Staff 
        public DbSet<TicketSubjectEntity> TicketSubjects { get; set; }
        public DbSet<TicketEntity> Tickets { get; set; }
        public DbSet<TicketCommentEntity> TicketComments { get; set; }

        // Files 
        public DbSet<FileEntity> Files { get; set; }


        private readonly IEventDispatcher _dispatcher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEventDispatcher eventDispatcher) : base(options)
        {
            _dispatcher = eventDispatcher;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //await DispatchDomainEventsAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());
        }

        private async Task DispatchDomainEventsAsync()
        {
            var entities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity);

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entities.ToList().ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    await _dispatcher.Dispatch(domainEvent, this);
                }
                catch
                {
                    // Обработка ошибок при диспетчеризации событий
                    // Можно добавить логирование или другие механизмы обработки ошибок
                    throw; // Можно выбрасывать исключение или обрабатывать ошибку по желанию
                }
            }
        }
    }
}
