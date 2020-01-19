using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Repositories
{
    public interface IRepository
    {
        public IUnitOfWork UnitOfWork { get; }
    }
}
