using Flsurf.Domain.Messanging.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Configuration
{
    public class UserToChatEntityConfiguration : IEntityTypeConfiguration<UserToChatEntity>
    {
        public void Configure(EntityTypeBuilder<UserToChatEntity> builder)
        {
            builder.HasKey(x => new { x.UserId, x.ChatId });

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.Chat)
                .WithMany()
                .HasForeignKey(x => x.ChatId);
        }
    }
}
