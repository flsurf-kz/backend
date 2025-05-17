using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Payment.Entities;
using Flsurf.Domain.Staff.Entities;
using Flsurf.Domain.User.Entities;
using Flsurf.Domain.Freelance.Entities;
using Flsurf.Domain.Messanging.Entities;

namespace Flsurf.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        ChangeTracker ChangeTracker { get; }

        // User 
        DbSet<UserEntity> Users { get; set; }
        DbSet<WarningEntity> UserWarnings { get; set; }
        DbSet<NotificationEntity> Notifications { get; set; }

        // payment 
        DbSet<TransactionProviderEntity> TransactionProviders { get; set; }
        DbSet<PaymentSystemEntity> PaymentSystems { get; set; }
        DbSet<TransactionEntity> Transactions { get; set; }
        DbSet<WalletEntity> Wallets { get; set; }
        DbSet<PaymentMethodEntity> PaymentMethods { get; set; }

        // Staff 
        DbSet<TicketEntity> Tickets { get; set; }
        DbSet<TicketCommentEntity> TicketComments { get; set; }
        DbSet<NewsEntity> News { get; set; }

        // Files 
        DbSet<FileEntity> Files { get; set; }


        // Freelance 
        public DbSet<BookmarkedJobEntity> BookmarkedJobs { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ClientProfileEntity> ClientProfiles { get; set; }
        public DbSet<ContestEntryEntity> ContestEntries { get; set; }
        public DbSet<ContestEntity> Contests { get; set; }
        public DbSet<ContractEntity> Contracts { get; set; }
        public DbSet<FreelancerProfileEntity> FreelancerProfiles { get; set; }
        public DbSet<FreelancerTeamEntity> FreelancerTeams { get; set; }
        public DbSet<FreelancerTeamInvitation> FreelancerTeamInvitations { get; set; }
        public DbSet<JobEntity> Jobs { get; set; }
        public DbSet<PortfolioProjectEntity> PortfolioProjects { get; set; }
        public DbSet<ProposalEntity> Proposals { get; set; }
        public DbSet<JobReviewEntity> Reviews { get; set; }
        public DbSet<SkillEntity> Skills { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<WorkSessionEntity> WorkSessions { get; set; }
        public DbSet<DisputeEntity> Disputes { get; set; }
        public DbSet<FreelancerProfileViewEntity> FreelancerProfileViews { get; set; }
        public DbSet<BonusEntity> Bonuses { get; set; }


        // messaging 
        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<ChatInvitationEntity> Invitations { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<MessageReadEntity> MessageReads { get; set; }
        public DbSet<UserToChatEntity> UserToChats { get; set; }
        public DbSet<SessionTicketEntity> SessionTickets { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity); 
    }
}
