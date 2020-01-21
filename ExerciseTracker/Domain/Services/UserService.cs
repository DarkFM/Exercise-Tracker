using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Repositories;
using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Responses.Exercise;
using ExerciseTracker.Domain.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<UserInfoResponse> AddUserAsync(NewUserRequest request)
        {
            var user = new User { UserName = request.Username };
            var savedUser = repository.AddUser(user);
            await repository.UnitOfWork.SaveChangesAsync();

            return new UserInfoResponse { _id = savedUser.Id, Username = savedUser.UserName };
        }

        public async Task<UserInfoResponse> GetUserByNameAsync(string username)
        {
            var user = await repository.GetUserByNameAsync(username);

            return new UserInfoResponse
            {
                _id = user.Id,
                Username = user.UserName
            };
        }

        public async Task<UserDetailsResponse> GetUserLogsAsync(Guid id, DateTime from = default, DateTime to = default)
        {
            var user = await repository.GetUserWithExercisesAsync(id, from, to);
            return new UserDetailsResponse
            {
                _id = user.Id,
                Username = user.UserName,
                Log = user.Exercises.Select(e => new ExerciseResponse
                {
                    Date = e.Date.ToUniversalTime().ToString("D"),
                    Description = e.Description,
                    Duration = e.Duration.TotalMinutes
                }).ToList()
            };
        }

        public async Task<IEnumerable<UserInfoResponse>> GetUsersAsync()
        {
            var users = await repository.GetUsersAsync();
            return users.Select(u => new UserInfoResponse
            {
                _id = u.Id,
                Username = u.UserName
            });
        }
    }
}
