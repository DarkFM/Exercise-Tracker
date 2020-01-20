using ExerciseTracker.Fixtures.Converters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ExerciseTracker.Fixtures.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder Seed<T>(this ModelBuilder builder, string file) where T : class
        {
            using var reader = new StreamReader(file);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new CustomTimeSpanJsonConverter());
            var json = JsonSerializer.Deserialize<T[]>(reader.ReadToEnd(), options);
            builder.Entity<T>().HasData(json);

            return builder;
        }
    }
}
