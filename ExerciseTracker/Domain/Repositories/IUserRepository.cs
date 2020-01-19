using ExerciseTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Repositories
{
    public interface IUserRepository : IRepository
    {
        Task<User> AddUser(User newUser);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(Guid id);
        Task<User> GetUserWithExercisesAsync(Guid id, DateTime from, DateTime to);
    }
}
