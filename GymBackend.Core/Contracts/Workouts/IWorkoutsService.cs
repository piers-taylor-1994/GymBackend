using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<RoutineSet?> GetRoutineAsync(string userId);
        Task<RoutineSet> AddRoutineAsync(string userId, List<string> exerciseIds);
        Task<RoutineSet> UpdateRoutineAsync(string id, List<SetUpdate> setList);
        Task<List<Routine>> GetRoutinesHistoryAsync(string userId);
        Task<RoutineSet> GetRoutineHistoryAsync(string id);
    }
}
