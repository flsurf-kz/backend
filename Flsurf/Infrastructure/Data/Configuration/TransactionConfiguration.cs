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

            // 👇 Конфигурация ValueObject: TransactionPropsEntity
            builder.OwnsOne(t => t.Props, props =>
            {
                props.Property(p => p.PaymentUrl).HasColumnName("PaymentUrl");
                props.Property(p => p.SuccessUrl).HasColumnName("SuccessUrl");
                props.Property(p => p.PaymentGateway).HasColumnName("PaymentGateway");

                // 👇 Конфигурация вложенного ValueObject: FeeContext
                props.OwnsOne(p => p.FeeContext, fee =>
                {
                    fee.Property(f => f.GatewayName).HasColumnName("FeeContext_GatewayName");
                    fee.Property(f => f.Provider).HasColumnName("FeeContext_Provider");
                });
            });
        }
    }
}
