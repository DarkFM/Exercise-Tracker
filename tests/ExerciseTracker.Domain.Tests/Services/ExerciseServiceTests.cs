using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Services;
using ExerciseTracker.Fixtures;
using ExerciseTracker.Fixtures.Factories;
using ExerciseTracker.Fixtures.TestDataAttributes;
using ExerciseTracker.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using System;
using Xunit;

namespace ExerciseTracker.Domain.Tests.Services
{
    public class ExerciseServiceTests : IClassFixture<TestDbContextFactory>, IDisposable
    {
        private readonly TestDbContext _dbContext;
        private readonly SqliteConnection _dbConnection;
        private readonly ExerciseService _sut;

        public ExerciseServiceTests(TestDbContextFactory factory)
        {
            _dbContext = factory.DbContextInstance;
            _dbConnection = factory.Connection;
            _sut = new ExerciseService(new ExerciseRepository(_dbContext), new UserRepository(_dbContext));
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }

        [Theory]
        [LoadUserData(1)]
        public void Should_Add_New_Exercise(User user)
        {
            var exerciseRequest = new NewExerciseRequest
            {
                Date = DateTime.UtcNow,
                UserId = user.Id,
                Duration = TimeSpan.FromMinutes(50).TotalMinutes,
                Description = "Go mountain biking with Janet"
            };

            var exerciseResponse = _sut.AddExerciseAsync(exerciseRequest).Result;

            Assert.NotNull(exerciseResponse);
            Assert.Equal(user.Id, exerciseResponse._id);
            Assert.Equal(user.UserName, exerciseResponse.Username);
            Assert.Equal(exerciseRequest.Duration, exerciseRequest.Duration);
            Assert.Equal(exerciseRequest.Date.ToString("D"), exerciseResponse.Date);
            Assert.Equal(exerciseRequest.Description, exerciseResponse.Description);
        }
    }
}
