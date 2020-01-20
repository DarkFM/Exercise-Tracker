using ExerciseTracker.Domain.Responses.Exercise;
using System.Collections.Generic;

namespace ExerciseTracker.Domain.Responses.User
{
    public class UserDetailsResponse : UserInfoResponse
    {
        public int Count => Log.Count;
        public IList<ExerciseResponse> Log { get; set; }
    }
}
