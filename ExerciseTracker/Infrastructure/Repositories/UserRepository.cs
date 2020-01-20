using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork => dbContext;

        public User AddUser(User newUser)
        {
            return dbContext.Add(newUser).Entity;
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await dbContext
                .Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByNameAsync(string username)
        {
            return await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == username);

        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserWithExercisesAsync(Guid id, DateTime from, DateTime to)
        {
            return await dbContext.Users
                .Include(u => u.Exercises)
                .Where(u => u.Exercises.Any(e => e.Date >= from && e.Date <= to))
                .SingleOrDefaultAsync(u => u.Id == id);
        }
    }
}
