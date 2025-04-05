using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Flsurf.Domain.Payment.Entities;

namespace Flsurf.Infrastructure.Data.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder
                .Property(e => e.Status)
                .HasConversion<string>();

            builder
                .Property(e => e.Flow)
                .HasConversion<string>();

            builder
                .Property(e => e.Type)
                .HasConversion<string>();

        }
    }
}
