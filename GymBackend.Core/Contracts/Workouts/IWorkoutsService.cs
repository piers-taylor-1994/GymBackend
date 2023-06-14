using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<RoutineSet?> GetRoutineAsync(Guid userId);
        Task<RoutineSet> AddRoutineAsync(Guid userId, List<string> exerciseIds);
        Task<RoutineSet> UpdateRoutineAsync(string id, List<SetUpdate> setList);
        Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId);
        Task<RoutineSet> GetRoutineHistoryAsync(string id);
    }
}
