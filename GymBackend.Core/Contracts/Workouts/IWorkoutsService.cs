using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<List<Guid>> SearchExercisesAsync(MuscleGroup muscle);
        Task<RoutineSet?> GetRoutineAsync(Guid userId);
        Task<RoutineSet> AddRoutineAsync(Guid userId, List<string> exerciseIds);
        Task<RoutineSet> AddRoutineAsync(Guid userId, List<NewSet> exercises);
        Task<RoutineSet> UpdateRoutineAsync(string id, List<SetUpdate> setList);
        Task DeleteSetFromRoutineAsync(Guid userId, string id);
        Task<Dictionary<Guid, int>> UpdateSetOrderAsync(Dictionary<Guid, int> setOrder);
        Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId);
        Task<RoutineSet> GetRoutineHistoryAsync(string id);
        Task<List<Set>> GetLastSetForExercisesAsync(Guid userId, List<string> exerciseIds);
        Task<List<MaxSet>> GetExerciseLeaderboardAsync(string exerciseId);
    }
}
