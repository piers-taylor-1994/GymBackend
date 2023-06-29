using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<RoutineSet?> GetRoutineAsync(Guid userId);
        Task<RoutineSet> AddRoutineAsync(Guid userId, List<ExerciseSet> exerciseList);
        //Task<RoutineSet> UpdateRoutineAsync(string id, List<SetUpdate> setList);
        Task DeleteSetFromRoutineAsync(Guid userId, string exerciseId);
        Task<Dictionary<Guid, int>> UpdateSetOrderAsync(Dictionary<Guid, int> setOrder);
        Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId);
        Task<RoutineSet> GetRoutineHistoryAsync(string id);
    }
}
