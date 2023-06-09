using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<RoutineSet> GetRoutineAsync(string userId);
        Task<RoutineSet> AddRoutineAsync(string userId, List<string> exerciseIds);
        Task<RoutineSet> UpdateRoutineAsync(string id, List<Set> setList);
    }
}
