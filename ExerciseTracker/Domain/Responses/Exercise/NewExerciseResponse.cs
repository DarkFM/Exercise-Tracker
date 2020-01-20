using System;

namespace ExerciseTracker.Domain.Responses.Exercise
{
    public class NewExerciseResponse : ExerciseResponse
    {
        public string Username { get; set; }
        public Guid _id { get; set; }
    }
}
