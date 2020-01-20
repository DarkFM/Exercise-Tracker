using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Fixtures.Factories;
using ExerciseTracker.Fixtures.TestDataAttributes;
using ExerciseTracker.Infrastructure.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExersiceTracker.Infrastructure.Tests
{
    public class ExerciseRepositoryTests : IClassFixture<TestDbContextFactory>
    {
        private readonly TestDbContextFactory _factory;

        public ExerciseRepositoryTests(TestDbContextFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [LoadUserData(0)]
        public void Should_add_new_exercise(User user)
        {
            var sut = new ExerciseRepository(_factory.DbContextInstance);
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
            sut.UnitOfWork.SaveChangesAsync();

            // get the result from the DB
            var savedExercise = _factory.DbContextInstance.Exercises.SingleOrDefault(e => e.Id == exercise.Id);

            Assert.NotNull(savedExercise);
            Assert.Equal(exercise, savedExercise, new EntityComparer<Exercise>());
        }

        #region Comparer Class

        private class EntityComparer<T> : System.Collections.Generic.IEqualityComparer<T>
        {
            private IEnumerable<PropertyInfo> _properties;
            public bool Equals(T expected, T actual)
            {
                _properties = typeof(T).GetProperties().Where(prop => !(prop.PropertyType is IEnumerable));
                foreach (var prop in _properties)
                {
                    var expectedValue = prop.GetValue(expected, null);
                    var actualValue = prop.GetValue(actual, null);
                    if (!expectedValue.Equals(actualValue))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(T obj)
            {
                return System.HashCode.Combine(_properties);
            }
        }

        #endregion
    }
}
