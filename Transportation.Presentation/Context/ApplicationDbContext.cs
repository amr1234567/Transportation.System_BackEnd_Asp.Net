﻿
using InfraStructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Transportation.Core.Identity;
using Transportation.Core.Models;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<ApplicationAdmin> Admins { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<BusStopManger> BusStopMangers { get; set; }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<UpcomingJourney> UpcomingJourneys { get; set; }
        public DbSet<JourneyHistory> Journeys { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.SeedAppData();
            builder.EditTables();
        }
    }
}
