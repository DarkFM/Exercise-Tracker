using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Responses.Exercise;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Services
{
    public interface IExerciseService
    {
        Task<NewExerciseResponse> AddExerciseAsync(NewExerciseRequest request);
    }
}
