﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.User.Entities;


namespace LMS.Infrastructure.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.Property(t => t.Surname)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
