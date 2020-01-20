using ExerciseTracker.Domain.Repositories;
using ExerciseTracker.Domain.Requests;
using ExerciseTracker.Domain.Responses.Exercise;
using System;
using System.Threading.Tasks;

namespace ExerciseTracker.Domain.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository exerciseRepository;
        private readonly IUserRepository userRepository;

        public ExerciseService(IExerciseRepository exerciseRepository, IUserRepository userRepository)
        {
            this.exerciseRepository = exerciseRepository;
            this.userRepository = userRepository;
        }

        public async Task<NewExerciseResponse> AddExerciseAsync(NewExerciseRequest request)
        {
            var exercise = exerciseRepository.Add(new Entities.Exercise
            {
                UserId = request.UserId,
                Date = request.Date,
                Duration = TimeSpan.FromMinutes(request.Duration),
                Description = request.Description
            });
            
            await exerciseRepository.UnitOfWork.SaveChangesAsync();

            return new NewExerciseResponse
            {
                _id = exercise.UserId,
                Username = (await userRepository.GetUserAsync(exercise.UserId)).UserName,
                Date = exercise.Date.ToUniversalTime().ToString("D"),
                Duration = exercise.Duration.TotalMinutes,
                Description = exercise.Description
            };
        }
    }
}
