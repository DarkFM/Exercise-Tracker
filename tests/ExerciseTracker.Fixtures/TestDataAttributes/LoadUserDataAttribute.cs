using ExerciseTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Xunit.Sdk;

namespace ExerciseTracker.Fixtures.TestDataAttributes
{
    public class LoadUserDataAttribute : DataAttribute
    {
        private readonly string _filePath;
        private readonly int _index;

        public LoadUserDataAttribute(int index)
        {
            _filePath = "./Data/users.json";
            _index = index;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) throw new ArgumentNullException(nameof(testMethod));

            if (!File.Exists(_filePath)) throw new FileNotFoundException($"File not found: {_filePath}");

            var json = File.ReadAllText(_filePath);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var user = JsonSerializer.Deserialize<User[]>(json, options)[_index];

            return new List<object[]> { new[] { user } };
        }
    }
}
