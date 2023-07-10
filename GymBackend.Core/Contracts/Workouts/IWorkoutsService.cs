using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<List<Guid>> SearchExercisesAsync(MuscleGroup muscle);
        Task<RoutineSet?> GetRoutineAsync(Guid userId);
        Task<RoutineSet> AddRoutineAsync(Guid userId, List<ExerciseSet> sets);
        Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId);
        Task<RoutineSet> GetRoutineHistoryAsync(string id);
        Task<List<Set>> GetLastSetForExercisesAsync(Guid userId, List<string> exerciseIds);
        Task<List<MaxSet>> GetExerciseLeaderboardAsync(string exerciseId);
    }
}
