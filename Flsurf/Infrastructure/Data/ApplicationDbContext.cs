using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Infrastructure.EventDispatcher;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Entities;

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

        // Freelance 
        public DbSet<BookmarkedJobEntity> BookmarkedJobs { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ClientProfileEntity> ClientProfiles { get; set; }
        public DbSet<ContestEntryEntity> ContestEntries { get; set; }
        public DbSet<ContestEntity> Contests { get; set; }
        public DbSet<ContractEntity> Contracts { get; set; }
        public DbSet<FreelancerProfileEntity> FreelancerProfiles { get; set; }
        public DbSet<FreelancerTeamEntity> FreelancerTeams { get; set; }
        public DbSet<JobEntity> Jobs { get; set; }
        public DbSet<PortfolioProjectEntity> PortfolioProjects { get; set; }
        public DbSet<ProposalEntity> Proposals { get; set; }
        public DbSet<JobReviewEntity> Reviews { get; set; }
        public DbSet<SkillEntity> Skills { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<WorkSnapshotEntity> WorkSnapshots { get; set; }

        // messaging 
        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<ChatInvitationEntity> Invitations { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<MessageReadEntity> MessageReads { get; set; }
        public DbSet<UserToChatEntity> UserToChats { get; set; }


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
