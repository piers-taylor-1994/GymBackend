using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercises>> GetExercisesAsync();
        Task<Routine> GetRoutineAsync(string userId);
    }
}
