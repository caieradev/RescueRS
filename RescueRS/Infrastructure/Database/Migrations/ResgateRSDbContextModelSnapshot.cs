﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;
using ResgateRS.Infrastructure.Database;

#nullable disable

namespace ResgateRS.Infrastructure.Database.Migrations
{
    [DbContext(typeof(ResgateRSDbContext))]
    partial class ResgateRSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ResgateRS.Domain.Application.Entities.RescueEntity", b =>
                {
                    b.Property<Guid>("RescueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("RAW(16)");

                    b.Property<int>("AnimalsNumber")
                        .HasColumnType("NUMBER(10)");

                    b.Property<int>("ChildrenNumber")
                        .HasColumnType("NUMBER(10)");

                    b.Property<Guid>("ConfirmedBy")
                        .HasColumnType("RAW(16)");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("DisabledNumber")
                        .HasColumnType("NUMBER(10)");

                    b.Property<int>("ElderlyNumber")
                        .HasColumnType("NUMBER(10)");

                    b.Property<double>("Latitude")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<double>("Longitude")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<DateTimeOffset>("RequestDateTime")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<Guid>("RequestedBy")
                        .HasColumnType("RAW(16)");

                    b.Property<DateTimeOffset?>("RescueDateTime")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<byte>("Rescued")
                        .HasColumnType("NUMBER(3)");

                    b.Property<int>("TotalPeopleNumber")
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("RescueId");

                    b.ToTable("Rescues");
                });

            modelBuilder.Entity("ResgateRS.Domain.Application.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("RAW(16)");

                    b.Property<string>("Cellphone")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<byte>("Rescuer")
                        .HasColumnType("NUMBER(3)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
