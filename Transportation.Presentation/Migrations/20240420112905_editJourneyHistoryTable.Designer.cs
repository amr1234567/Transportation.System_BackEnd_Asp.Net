﻿// <auto-generated />
using System;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InfraStructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240420112905_editJourneyHistoryTable")]
    partial class editJourneyHistoryTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Security")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BusStopsRelations", b =>
                {
                    b.Property<string>("StartBusStopId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DestinationStopId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("StartBusStopId", "DestinationStopId");

                    b.HasIndex("DestinationStopId");

                    b.ToTable("BusStopsRelations", "App");
                });

            modelBuilder.Entity("Core.Identity.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", "Security");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Core.Models.Bus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BusStopMangerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<int>("NumberOfSeats")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BusStopMangerId");

                    b.ToTable("Buses", "App");
                });

            modelBuilder.Entity("Core.Models.JourneyHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("BusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("DestinationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("LeavingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("StartBusStopId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("TicketPrice")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("BusId");

                    b.HasIndex("DestinationId");

                    b.HasIndex("StartBusStopId");

                    b.ToTable("Journeys", "App");
                });

            modelBuilder.Entity("Core.Models.Seat", b =>
                {
                    b.Property<Guid>("SeatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<int>("SeatNum")
                        .HasColumnType("int");

                    b.HasKey("SeatId");

                    b.HasIndex("BusId");

                    b.ToTable("Seats", "App");
                });

            modelBuilder.Entity("Core.Models.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ConsumerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DestinationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DestinationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("JourneyHistoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("JourneyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LeavingTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<bool>("ReservedOnline")
                        .HasColumnType("bit");

                    b.Property<int>("SeatNum")
                        .HasColumnType("int");

                    b.Property<string>("StartBusStopId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartBusStopName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpcomingJourneyId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("JourneyHistoryId");

                    b.HasIndex("UpcomingJourneyId");

                    b.ToTable("Tickets", "App");
                });

            modelBuilder.Entity("Core.Models.UpcomingJourney", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("BusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DestinationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("JourneyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LeavingTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumberOfAvailableTickets")
                        .HasColumnType("int");

                    b.Property<string>("StartBusStopId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("TicketPrice")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("BusId");

                    b.HasIndex("DestinationId");

                    b.HasIndex("StartBusStopId");

                    b.ToTable("UpcomingJourneys", "App");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles", "Security");

                    b.HasData(
                        new
                        {
                            Id = "35561bfe-d346-4b70-8380-b15d23edbe2e",
                            ConcurrencyStamp = "1",
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = "a7d62d27-150f-4165-961e-e5095e09ddb1",
                            ConcurrencyStamp = "2",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "47470826-cdc6-4eb0-9ebe-75f10c89c1da",
                            ConcurrencyStamp = "3",
                            Name = "Manager",
                            NormalizedName = "MANAGER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", "Security");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", "Security");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", "Security");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", "Security");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", "Security");
                });

            modelBuilder.Entity("Core.Identity.ApplicationAdmin", b =>
                {
                    b.HasBaseType("Core.Identity.User");

                    b.ToTable("Admins", "Security");
                });

            modelBuilder.Entity("Core.Identity.ApplicationUser", b =>
                {
                    b.HasBaseType("Core.Identity.User");

                    b.ToTable("Users", "Security");
                });

            modelBuilder.Entity("Core.Identity.BusStopManger", b =>
                {
                    b.HasBaseType("Core.Identity.User");

                    b.ToTable("Managers", "App");
                });

            modelBuilder.Entity("BusStopsRelations", b =>
                {
                    b.HasOne("Core.Identity.BusStopManger", null)
                        .WithMany()
                        .HasForeignKey("DestinationStopId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("FK_BusStopsRelations_DestinationStop");

                    b.HasOne("Core.Identity.BusStopManger", null)
                        .WithMany()
                        .HasForeignKey("StartBusStopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_BusStopsRelations_StartBusStop");
                });

            modelBuilder.Entity("Core.Models.Bus", b =>
                {
                    b.HasOne("Core.Identity.BusStopManger", null)
                        .WithMany("Buses")
                        .HasForeignKey("BusStopMangerId");
                });

            modelBuilder.Entity("Core.Models.JourneyHistory", b =>
                {
                    b.HasOne("Core.Models.Bus", "Bus")
                        .WithMany()
                        .HasForeignKey("BusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Identity.BusStopManger", "Destination")
                        .WithMany()
                        .HasForeignKey("DestinationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Identity.BusStopManger", "StartBusStop")
                        .WithMany()
                        .HasForeignKey("StartBusStopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");

                    b.Navigation("Destination");

                    b.Navigation("StartBusStop");
                });

            modelBuilder.Entity("Core.Models.Seat", b =>
                {
                    b.HasOne("Core.Models.Bus", "Bus")
                        .WithMany("seats")
                        .HasForeignKey("BusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");
                });

            modelBuilder.Entity("Core.Models.Ticket", b =>
                {
                    b.HasOne("Core.Models.JourneyHistory", null)
                        .WithMany("Tickets")
                        .HasForeignKey("JourneyHistoryId");

                    b.HasOne("Core.Models.UpcomingJourney", null)
                        .WithMany("Ticket")
                        .HasForeignKey("UpcomingJourneyId");
                });

            modelBuilder.Entity("Core.Models.UpcomingJourney", b =>
                {
                    b.HasOne("Core.Models.Bus", "Bus")
                        .WithMany()
                        .HasForeignKey("BusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Identity.BusStopManger", "Destination")
                        .WithMany()
                        .HasForeignKey("DestinationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Identity.BusStopManger", "StartBusStop")
                        .WithMany()
                        .HasForeignKey("StartBusStopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");

                    b.Navigation("Destination");

                    b.Navigation("StartBusStop");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Core.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Core.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Core.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Identity.ApplicationAdmin", b =>
                {
                    b.HasOne("Core.Identity.User", null)
                        .WithOne()
                        .HasForeignKey("Core.Identity.ApplicationAdmin", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Identity.ApplicationUser", b =>
                {
                    b.HasOne("Core.Identity.User", null)
                        .WithOne()
                        .HasForeignKey("Core.Identity.ApplicationUser", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Identity.BusStopManger", b =>
                {
                    b.HasOne("Core.Identity.User", null)
                        .WithOne()
                        .HasForeignKey("Core.Identity.BusStopManger", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.Models.Bus", b =>
                {
                    b.Navigation("seats");
                });

            modelBuilder.Entity("Core.Models.JourneyHistory", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Core.Models.UpcomingJourney", b =>
                {
                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Core.Identity.BusStopManger", b =>
                {
                    b.Navigation("Buses");
                });
#pragma warning restore 612, 618
        }
    }
}
