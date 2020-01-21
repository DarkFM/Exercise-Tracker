using ExerciseTracker.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExerciseTracker.Domain.Extensions
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services
                .AddScoped<IExerciseService, ExerciseService>()
                .AddScoped<IUserService, UserService>();
            
            return services;
        }
    }
}
