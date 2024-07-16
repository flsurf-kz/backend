using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Infrastructure.Data.Configuration
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<PurchaseEntity>
    {
        public void Configure(EntityTypeBuilder<PurchaseEntity> builder)
        {
            builder
                .Property(e => e.Status)
                .HasConversion<string>();
            builder
                .Property(e => e.Currency)
                .HasConversion<string>();
        }
    }
}
