using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Fixtures;
using ExerciseTracker.Fixtures.Comparers;
using ExerciseTracker.Fixtures.Factories;
using ExerciseTracker.Fixtures.TestDataAttributes;
using ExerciseTracker.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using Xunit;

namespace ExerciseTracker.Infrastructure.Tests
{
    public class ExerciseRepositoryTests : IClassFixture<TestDbContextFactory>, IDisposable
    {
        private readonly SqliteConnection _dbConnection;
        private readonly TestDbContext _dbContext;

        public ExerciseRepositoryTests(TestDbContextFactory factory)
        {
            _dbContext = factory.DbContextInstance;
            _dbConnection = factory.Connection;
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_add_new_exercise(User user)
        {
            var sut = new ExerciseRepository(_dbContext);
            var exercise = new Exercise
            {
                Date = DateTime.Now.AddHours(1),
                Description = "Do 100 pushups",
                Duration = TimeSpan.FromMinutes(50),
                UserId = user.Id,
                Id = Guid.NewGuid()
            };

            // add and save to db
            sut.Add(exercise);
            sut.UnitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

            // get the result from the DB
            var savedExercise = _dbContext.Exercises.SingleOrDefault(e => e.Id == exercise.Id);

            Assert.NotNull(savedExercise);
            Assert.Equal(exercise, savedExercise, new EntityComparer<Exercise>());
        }

    }
}
