using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Fixtures;
using ExerciseTracker.Fixtures.TestDataAttributes;
using ExerciseTracker.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace ExerciseTracker.Infrastructure.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SqliteConnection DbConnection;
        private readonly TestDbContext _dbContext;

        public UserRepositoryTests()
        {
            DbConnection = new SqliteConnection("DataSource=:memory:");
            DbConnection.Open();
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(DbConnection)
                .Options;

            using (var context = new TestDbContext(contextOptions))
            {
                context.Database.EnsureCreated();
            }

            _dbContext = new TestDbContext(contextOptions);
        }

        [Fact]
        public void Should_save_new_user()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser-Name/\""
            };

            var sut = new UserRepository(_dbContext);
            sut.AddUser(user);
            sut.UnitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

            // verify result
            var savedUser = _dbContext.Users.SingleOrDefault(u => u.Id == user.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser, new EntityComparer<User>());
        }

        [Theory]
        [LoadUserData]
        public void Should_return_list_of_all_users(User[] users)
        {
            var sut = new UserRepository(_dbContext);
            var storedUsers = sut.GetUsersAsync().GetAwaiter().GetResult();

            Assert.NotEmpty(storedUsers);
            foreach (var user in storedUsers)
            {
                Assert.Contains(user, users, new EntityComparer<User>());
            }
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_return_user_given_the_id(User user)
        {
            var sut = new UserRepository(_dbContext);
            var savedUser = sut.GetUserAsync(user.Id).GetAwaiter().GetResult();
            
            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser, new EntityComparer<User>());
        }
        
        [Theory]
        [LoadUserData(1)]
        public void Should_return_user_given_the_name(User user)
        {
            var sut = new UserRepository(_dbContext);
            var savedUser = sut.GetUserByNameAsync(user.UserName).GetAwaiter().GetResult();

            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser, new EntityComparer<User>());
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_return_user_and_the_exercises(User user)
        {
            var sut = new UserRepository(_dbContext);
            var savedUser = sut.GetUserWithExercisesAsync(user.Id, new DateTime(2020, 01, 16), new DateTime(2020, 01, 20)).Result;

            Assert.NotNull(savedUser);
            Assert.NotEmpty(savedUser.Exercises);
            Assert.Equal(2, savedUser.Exercises.Count);
        }

        public void Dispose()
        {
            DbConnection.Close();
        }
    }
}
