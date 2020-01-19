using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Infrastructure
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Exercise>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Exercise>()
                .HasOne<User>()
                .WithMany(x => x.Exercises)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.Date)
                .HasConversion<long>();

            modelBuilder.Entity<Exercise>()
                .Property(e => e.Duration)
                .HasConversion<long>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
