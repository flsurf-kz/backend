using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.Data.Configuration;
using Flsurf.Application.Common.Events;
using Flsurf.Infrastructure.EventStore;

namespace Flsurf.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        // User 
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<WarningEntity> UserWarnings { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<ConnectedAccountEntity> ConnectedAccounts { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<SessionTicketEntity> SessionTickets { get; set; }


        // payment 
        public DbSet<TransactionProviderEntity> TransactionProviders { get; set; }
        public DbSet<PaymentSystemEntity> PaymentSystems { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<WalletEntity> Wallets { get; set; }
        public DbSet<PaymentMethodEntity> PaymentMethods { get; set; }


        // Staff 
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
        public DbSet<DisputeStatusHistory> DisputeStatuses { get; set; }
        public DbSet<FreelancerProfileEntity> FreelancerProfiles { get; set; }
        public DbSet<FreelancerTeamEntity> FreelancerTeams { get; set; }
        public DbSet<JobEntity> Jobs { get; set; }
        public DbSet<PortfolioProjectEntity> PortfolioProjects { get; set; }
        public DbSet<ProposalEntity> Proposals { get; set; }
        public DbSet<JobReviewEntity> Reviews { get; set; }
        public DbSet<SkillEntity> Skills { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<FreelancerTeamInvitation> FreelancerTeamInvitations { get; set; }
        public DbSet<WorkSessionEntity> WorkSessions { get; set; }
        public DbSet<DisputeEntity> Disputes { get; set; }
        public DbSet<FreelancerProfileViewEntity> FreelancerProfileViews { get; set; }

        // messaging 
        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<ChatInvitationEntity> Invitations { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<MessageReadEntity> MessageReads { get; set; }
        public DbSet<UserToChatEntity> UserToChats { get; set; }


        private readonly IEventDispatcher _dispatcher;
        private readonly IEventStore _eventStore; 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEventDispatcher eventDispatcher, IEventStore eventStore) : base(options)
        {
            _dispatcher = eventDispatcher;
            // god forgive me for this sin 
            _eventStore = eventStore; 
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Собираем все события, они не будут разделены на две категории из за запарности 
            var eventsEntites = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents?.Any() == true)
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = eventsEntites
                .SelectMany(e => e.DomainEvents)
                .ToList();

            eventsEntites.ForEach(e => e.ClearDomainEvents());

            // Вызываем доменные обработчики (как раньше)
            await DispatchDomainEventsAsync(domainEvents);

            // Сохраняем (commit) доменные изменения
            var result = await base.SaveChangesAsync(cancellationToken);

            // Сохраняем эти события в таблицу Outbox (или EventStore)
            // Здесь можно сериализовать событие и добавить в OutboxIntegrationEvents
            foreach (var @event in domainEvents)
            {
                await _eventStore.StoreEvent(@event, isIntegrationEvent: true);
            }
            // теперь эти события в паралельном обработчике. 

            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());

            builder.ApplyEnumToStringConversion();
        }

        private async Task DispatchDomainEventsAsync(List<BaseEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                try
                {
                    await _dispatcher.DispatchDomainEventAsync(domainEvent, this);
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
