using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Services;
using ExerciseTracker.Fixtures;
using ExerciseTracker.Fixtures.Factories;
using ExerciseTracker.Fixtures.TestDataAttributes;
using ExerciseTracker.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using Xunit;

namespace ExerciseTracker.Domain.Tests.Services
{
    public class UserServiceTests : IClassFixture<TestDbContextFactory>, IDisposable
    {
        private readonly TestDbContext _dbContext;
        private readonly SqliteConnection _dbConnection;
        private readonly UserService _sut;

        public UserServiceTests(TestDbContextFactory factory)
        {
            _dbContext = factory.DbContextInstance;
            _dbConnection = factory.Connection;
            _sut = new UserService(new UserRepository(_dbContext));
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }

        [Theory]
        [InlineData("user-one")]
        public void Should_add_new_users(string username)
        {
            var userRequest = new NewUserRequest { Username = username };
            var result = _sut.AddUserAsync(userRequest).Result;

            Assert.Equal(username, result.Username);
            Assert.NotEqual(Guid.Empty, result._id);
        }

        [Theory]
        [LoadUserData(1)]
        public void Should_get_user_by_username(User user)
        {
            var result = _sut.GetUserByNameAsync(user.UserName).Result;

            Assert.Equal(user.Id, result._id);
            Assert.Equal(user.UserName, result.Username);
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_get_all_user_logs(User user)
        {
            var result = _sut.GetUserLogsAsync(user.Id).Result;

            Assert.Equal(user.Id, result._id);
            Assert.Equal(user.UserName, result.Username);
            Assert.NotEmpty(result.Log);
            Assert.Equal(3, result.Count);
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_get_all_user_logs_filtered_by_date(User user)
        {
            var result = _sut.GetUserLogsAsync(user.Id, to: new DateTime(2020, 1, 15)).Result;

            Assert.Equal(user.Id, result._id);
            Assert.Equal(user.UserName, result.Username);
            Assert.NotEmpty(result.Log);
            Assert.Equal(1, result.Count);
        }

        [Theory]
        [LoadUserData]
        public void Should_get_all_users_in_db(User[] users)
        {
            var result = _sut.GetUsersAsync().Result.ToList();

            Assert.Equal(2, result.Count);
            foreach (var user in users)
            {
                Assert.Contains(result, u => u.Username == user.UserName);
            }
        }
    }
}
