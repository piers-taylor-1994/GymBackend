using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<List<Guid>> SearchExercisesAsync(MuscleGroup muscle);
        Task<RoutineSet?> GetRoutineAsync(Guid userId, int submissionType);
        Task<Guid> AddRoutineAsync(Guid userId, List<ExerciseSets> exerciseSets, int submissionType);
        Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId, int submissionType);
        Task<RoutineSet> GetRoutineHistoryAsync(string id, int submissionType);
        Task<List<Set>> GetLastSetForExercisesAsync(Guid userId, List<string> exerciseIds);
        Task<List<MaxSet>> GetExerciseLeaderboardAsync(string exerciseId);
        Task<RoutineTemplate> AddRoutineTemplateAsync(Guid userId, string name, List<string> exerciseIds);
        Task<List<RoutineTemplate>> GetRoutineTemplatesAsync(Guid userId);
        Task<List<Exercise>> GetRoutineTemplateSetsAsync(Guid userId, string id);
        Task<List<RoutineTemplate>> UpdateRoutineTemplateAsync(Guid userId, string id, string name, List<string> exerciseIds);
        Task<List<RoutineTemplate>> DeleteRoutineTemplateAsync(Guid userId, string id);
        Task<WorkoutsCount> GetWorkoutsCountAsync(Guid userId);
        Task<Exercise> AddExerciseAsync(string name, List<MuscleGroup> muscles);
        Task<List<Routine>> GetMostRecentWorkoutsAsync();
        Task<Guid> ResurrectGhostAsync(Guid userId, Guid routineId, DateTime date);
    }
}
