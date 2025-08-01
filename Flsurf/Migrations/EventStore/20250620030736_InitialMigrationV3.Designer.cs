﻿// <auto-generated />
using System;
using Flsurf.Infrastructure.EventStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Flsurf.Migrations.EventStore
{
    [DbContext(typeof(EventStoreContext))]
    [Migration("20250620030736_InitialMigrationV3")]
    partial class InitialMigrationV3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Flsurf.Infrastructure.EventStore.StoredEvent", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ByUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("ErrorData")
                        .HasColumnType("jsonb");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FailedCounter")
                        .HasColumnType("integer");

                    b.Property<bool>("IsIntegrationEvent")
                        .HasColumnType("boolean");

                    b.Property<bool>("ProcessError")
                        .HasColumnType("boolean");

                    b.Property<bool>("Processed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("EventId");

                    b.HasIndex("EventId")
                        .HasDatabaseName("IX_EventId");

                    b.HasIndex("EventType")
                        .HasDatabaseName("IX_EventType");

                    b.ToTable("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
