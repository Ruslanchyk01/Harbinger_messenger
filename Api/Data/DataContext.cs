using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Converters;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        // protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        // {
        //     base.ConfigureConventions(configurationBuilder);

        //     configurationBuilder.Properties<DateOnly>()
        //         .HaveConversion<DateOnlyEFConverter>()
        //         .HaveColumnType("date");

        // }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

             builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}