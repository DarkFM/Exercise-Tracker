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
    public class UserRepositoryTests : IClassFixture<TestDbContextFactory>, IDisposable
    {
        private readonly SqliteConnection _dbConnection;
        private readonly TestDbContext _dbContext;
        private readonly UserRepository _sut;

        public UserRepositoryTests(TestDbContextFactory factory)
        {
            _dbConnection = factory.Connection;
            _dbContext = factory.DbContextInstance;
            _sut = new UserRepository(_dbContext);
        }

        [Fact]
        public void Should_save_new_user()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "TestUser-Name/\""
            };

            
            _sut.AddUser(user);
            _sut.UnitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

            // verify result
            var savedUser = _dbContext.Users.SingleOrDefault(u => u.Id == user.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser, new EntityComparer<User>());
        }

        [Theory]
        [LoadUserData]
        public void Should_return_list_of_all_users(User[] users)
        {
            
            var storedUsers = _sut.GetUsersAsync().GetAwaiter().GetResult();

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
            
            var savedUser = _sut.GetUserAsync(user.Id).GetAwaiter().GetResult();
            
            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser, new EntityComparer<User>());
        }
        
        [Theory]
        [LoadUserData(1)]
        public void Should_return_user_given_the_name(User user)
        {
            
            var savedUser = _sut.GetUserByNameAsync(user.UserName).GetAwaiter().GetResult();

            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser, new EntityComparer<User>());
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_return_user_and_the_exercises(User user)
        {
            
            var savedUser = _sut.GetUserWithExercisesAsync(user.Id, from: new DateTime(2020, 01, 16), to: new DateTime(2020, 01, 20)).Result;

            Assert.NotNull(savedUser);
            Assert.NotEmpty(savedUser.Exercises);
            Assert.Equal(2, savedUser.Exercises.Count());
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_return_user_and_exercises_given_only_from_date_is_given(User user)
        {
            var savedUser = _sut.GetUserWithExercisesAsync(user.Id, from: new DateTime(2020, 01, 20)).Result;

            Assert.NotNull(savedUser);
            Assert.NotEmpty(savedUser.Exercises);
            Assert.Single(savedUser.Exercises);
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_return_user_and_exercises_given_only_to_date_is_given(User user)
        {
            var savedUser = _sut.GetUserWithExercisesAsync(user.Id, to: new DateTime(2020, 01, 17)).Result;

            Assert.NotNull(savedUser);
            Assert.NotEmpty(savedUser.Exercises);
            Assert.Equal(2, savedUser.Exercises.Count());
        }

        [Theory]
        [LoadUserData(1)]
        public void Should_return_user_and_exercises_given_no_date_is_given(User user)
        {
            var savedUser = _sut.GetUserWithExercisesAsync(user.Id).Result;

            Assert.NotNull(savedUser);
            Assert.NotEmpty(savedUser.Exercises);
            Assert.Equal(2, savedUser.Exercises.Count());
        }

        public void Dispose()
        {
            _dbConnection.Close();
        }
    }
}
