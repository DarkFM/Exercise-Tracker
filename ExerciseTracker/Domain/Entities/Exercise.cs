using System;

namespace ExerciseTracker.Domain.Entities
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
}
