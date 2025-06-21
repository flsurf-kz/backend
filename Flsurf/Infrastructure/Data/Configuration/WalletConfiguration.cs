using Flsurf.Domain.User.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Infrastructure.Data.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<WalletEntity>
    {
        public void Configure(EntityTypeBuilder<WalletEntity> builder)
        {
            //builder.Property(t => t.RowVersion).IsRowVersion(); 
        }
    }
}
