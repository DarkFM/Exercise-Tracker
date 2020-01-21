using ExerciseTracker.Domain.Repositories;
using ExerciseTracker.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ExerciseTracker.Infrastructure.Extensions
{
    public static class DependenciesRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped<IExerciseRepository, ExerciseRepository>()
                .AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
