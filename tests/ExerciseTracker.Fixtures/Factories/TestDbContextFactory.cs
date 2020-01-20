using ExerciseTracker.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExerciseTracker.Fixtures.Factories
{
    public class TestDbContextFactory
    {
        public readonly TestDbContext DbContextInstance;

        public TestDbContextFactory()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new TestDbContext(contextOptions))
            {
                context.Database.EnsureCreated();
            }

            DbContextInstance = new TestDbContext(contextOptions);
        }
    }
}
