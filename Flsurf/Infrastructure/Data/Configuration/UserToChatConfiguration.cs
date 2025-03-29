using Flsurf.Domain.Messanging.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Flsurf.Infrastructure.Data.Configuration
{
    public class UserToChatEntityConfiguration : IEntityTypeConfiguration<UserToChatEntity>
    {
        public void Configure(EntityTypeBuilder<UserToChatEntity> builder)
        {
            // 👥 Составной первичный ключ
            builder.HasKey(x => new { x.UserId, x.ChatId });

            // 🛠 Прочие настройки (опционально, например, названия колонок)
            builder.Property(x => x.NotificationsDisabled)
                   .HasDefaultValue(false);

            builder.Property(x => x.Bookmarked)
                   .HasDefaultValue(false);
        }
    }
}
