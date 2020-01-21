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

        public async Task<User> GetUserWithExercisesAsync(Guid id, int? limit = null, DateTime from = default, DateTime to = default)
        {
            if (to == default)
                to = DateTime.UtcNow.AddDays(1000);

            if (from > to)
                throw new ArgumentException($"{nameof(from)} cannot be larger than {nameof(to)}");

            var user = await dbContext.Users
                .Where(u => u.Id == id)
                .Include(u => u.Exercises)
                .Select(u => new User
                {
                    Exercises = u.Exercises.Where(e => e.Date >= from && e.Date <= to),
                    Id = u.Id,
                    UserName = u.UserName
                })
                .SingleOrDefaultAsync();

            user.Exercises = user.Exercises
                .OrderBy(e => e.Date)
                .TakeWhile((_, idx) => idx != limit)
                .ToList();

            return user;
        }
    }
}
