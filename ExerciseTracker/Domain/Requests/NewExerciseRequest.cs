using System;

namespace ExerciseTracker.Domain.Requests
{
    public class NewExerciseRequest
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
