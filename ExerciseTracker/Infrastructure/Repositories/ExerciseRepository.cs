using ExerciseTracker.Domain.Entities;
using ExerciseTracker.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Infrastructure.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly AppDbContext dbContext;

        public ExerciseRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork => dbContext;

        public Exercise Add(Exercise exercise)
        {
            return dbContext.Exercises.Add(exercise).Entity;
        }
    }
}
