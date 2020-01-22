using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Responses.Exercise;
using ExerciseTracker.Domain.Responses.User;
using ExerciseTracker.Fixtures.Factories;
using ExerciseTracker.Fixtures.TestDataAttributes;
using ExerciseTracker.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ExerciseTracker.Integration.Tests
{
    public class AppControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AppControllerTests(InMemoryApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/")]
        public async Task Smoke_test_on_base_endpoint(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/api/exercise/users")]
        public async Task Should_return_all_registered_users(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<UserInfoResponse[]>(json, serializerOptions);

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Length);
        }

        [Theory]
        [LoadUserData(0)]
        public async Task Should_return_user_with_all_log(User user)
        {
            var response = await _client.GetAsync($"/api/exercise/log?userId={user.Id}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<UserDetailsResponse>(json, serializerOptions);

            Assert.Equal(user.UserName, result.Username);
            Assert.Equal(3, result.Count);
        }

        [Theory]
        [LoadUserData(0)]
        public async Task Should_return_user_with_filtered_logs(User user)
        {
            string from = "2020-1-17", to = "2020-1-20";
            int limit = 1;

            var response = await _client
                .GetAsync($"/api/exercise/log?userId={user.Id}&from={from}&to={to}&limit={limit}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<UserDetailsResponse>(json, serializerOptions);

            Assert.Equal(user.Id, result._id);
            Assert.Equal(user.UserName, result.Username);
            Assert.Equal(1, result.Count);
            Assert.Equal(TimeSpan.FromMinutes(25).TotalMinutes, result.Log[0].Duration);
        }

        [Theory]
        [LoadUserData(0)]
        public async Task Should_return_error_when_from_date_is_larger_than_to_date(User user)
        {
            string from = "2020-1-20", to = "2020-1-17";
            int limit = 1;

            var response = await _client
                .GetAsync($"/api/exercise/log?userId={user.Id}&from={from}&to={to}&limit={limit}");

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ErrorModel>(json, serializerOptions);

            Assert.Equal(400, result.Status);
            Assert.Contains("from cannot be larger than to", result.Errors["errors"][0], StringComparison.InvariantCultureIgnoreCase);
        }


        [Theory]
        [LoadUserData(1)]
        public async Task Should_return_newly_added_exercise(User user)
        {
            var newExercise = new NewExerciseRequest
            {
                UserId = user.Id,
                Description = "Test description",
                Duration = TimeSpan.FromMinutes(120).TotalMinutes
            };

            var formContent = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create(nameof(newExercise.UserId), newExercise.UserId.ToString()),
                KeyValuePair.Create(nameof(newExercise.Description), newExercise.Description),
                KeyValuePair.Create(nameof(newExercise.Duration), newExercise.Duration.ToString()),
            };

            var httpMessage = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync("/api/exercise/add", httpMessage);

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<NewExerciseResponse>(json, serializerOptions);

            Assert.Equal(user.Id, result._id);
            Assert.Equal(user.UserName, result.Username);
            Assert.Equal(newExercise.Description, result.Description);
            Assert.Equal(newExercise.Duration, result.Duration);
            Assert.NotEqual(default, DateTime.Parse(result.Date));
            Assert.Equal(DateTime.UtcNow.Day, DateTime.Parse(result.Date).Day);
        }

        [Theory]
        [LoadUserData(1)]
        public async Task Should_return_error_when_date_is_not_valid(User user)
        {
            var newExercise = new
            {
                UserId = user.Id,
                Description = "Test description",
                Duration = TimeSpan.FromMinutes(120).TotalMinutes,
                Date = "2020-20-100"
            };

            var formContent = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create(nameof(newExercise.UserId), newExercise.UserId.ToString()),
                KeyValuePair.Create(nameof(newExercise.Description), newExercise.Description),
                KeyValuePair.Create(nameof(newExercise.Duration), newExercise.Duration.ToString()),
                KeyValuePair.Create(nameof(newExercise.Date), newExercise.Date),
            };

            var httpMessage = new FormUrlEncodedContent(formContent);
            var response = await _client.PostAsync("/api/exercise/add", httpMessage);

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ErrorModel>(json, serializerOptions);

            Assert.Equal(400, result.Status);
            Assert.Contains("Invalid date format", result.Errors["Date"][0], StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [InlineData("/api/exercise/new-user")]
        public async Task Returns_newly_created_user(string endpoint)
        {
            var newUser = new NewUserRequest { Username = "aksjdskdjf" };

            var formContent = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create(nameof(newUser.Username), newUser.Username),
            };

            var httpContent = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync(endpoint, httpContent);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<UserInfoResponse>(json, serializerOptions);

            Assert.NotEqual(default, result._id);
            Assert.Equal(newUser.Username, result.Username);
        }

        [Theory]
        [LoadUserData(0)]
        public async Task Should_return_error_when_user_already_exists(User user)
        {
            var newUser = new NewUserRequest { Username = user.UserName };

            var formContent = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create(nameof(newUser.Username), newUser.Username),
            };

            var httpContent = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync("/api/exercise/new-user", httpContent);

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());

            var json = await response.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<IDictionary<string, string[]>>(json, serializerOptions);

            Assert.Contains("username already taken", result["username"][0], StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
