﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Flsurf.Infrastructure.Data.Configuration
{
    public static class EnumToStringConverterConfiguration
    {
        public static void ApplyEnumToStringConversion(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Пропускаем owned-сущности
                if (entityType.IsOwned())
                {
                    continue;
                }

                var enumProperties = entityType.ClrType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.IsEnum ||
                                (Nullable.GetUnderlyingType(p.PropertyType)?.IsEnum ?? false));

                foreach (var property in enumProperties)
                {
                    var enumType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var converterType = typeof(EnumToStringConverter<>).MakeGenericType(enumType);
                    var converter = (ValueConverter)Activator.CreateInstance(converterType)!;

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }
        }
    }
}
