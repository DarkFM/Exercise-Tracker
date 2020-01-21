using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Responses.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Services
{
    public interface IUserService
    {
        Task<UserInfoResponse> AddUserAsync(NewUserRequest request);
        Task<IEnumerable<UserInfoResponse>> GetUsersAsync();
        Task<UserInfoResponse> GetUserByNameAsync(string username);
        Task<UserDetailsResponse> GetUserLogsAsync(Guid id, int? limit, DateTime from, DateTime to);
    }
}
