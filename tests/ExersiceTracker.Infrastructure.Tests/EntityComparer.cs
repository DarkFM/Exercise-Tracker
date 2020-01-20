using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExerciseTracker.Infrastructure.Tests
{
    public class EntityComparer<T> : IEqualityComparer<T>
    {
        private IEnumerable<PropertyInfo> _properties;
        public bool Equals(T expected, T actual)
        {
            _properties = typeof(T).GetProperties().Where(prop => !typeof(IEnumerable).IsAssignableFrom(prop.PropertyType));
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
}
