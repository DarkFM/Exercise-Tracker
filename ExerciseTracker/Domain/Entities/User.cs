using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
    }
}
