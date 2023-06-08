using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<RoutineView> GetRoutineAsync(string userId);
        Task<RoutineView> AddRoutineAsync(string userId, List<string> exerciseIds);
    }
}
