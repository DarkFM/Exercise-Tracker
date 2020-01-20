using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Fixtures.Extensions;
using ExerciseTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExerciseTracker.Fixtures
{
    public class TestDbContext : AppDbContext
    {
        public TestDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Seed<Exercise>("./Data/exercises.json")
                .Seed<User>("./Data/users.json");

        }
    }
}
