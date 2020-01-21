using ExerciseTracker.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExerciseTracker.Fixtures.Factories
{
    public class TestDbContextFactory
    {
        public readonly SqliteConnection Connection;

        public TestDbContextFactory()
        {
            Connection = new SqliteConnection("DataSource=:memory:");
        }

        public TestDbContext DbContextInstance
        {
            get
            {
                Connection.Open();

                var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(Connection)
                .Options;

                using (var context = new TestDbContext(options))
                {
                    context.Database.EnsureCreated();
                }
                return new TestDbContext(options);
            }
        }
    }
}