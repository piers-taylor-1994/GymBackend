using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsStorage
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<List<Guid>> GetAllSearchExercisesAsync(MuscleGroup muscle);
        Task<Routine?> GetRoutineAsync(Guid userId, DateTime date);
        Task<List<Set>> GetSetExerciseIdOrderByRoutineIdAsync(Guid routineId);
        Task<List<SetArray>> GetSetsArrayBySetId(Guid setId);
        Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date);
        Task UpdateRoutineTimeAsync(Guid id, Guid userId, DateTime date);
        Task AddExercisesToSetAsync(Guid id, Guid routineId, Guid exerciseId, int order);
        Task AddExerciseSetFromArrayAsync(Guid setId, float weight, int sets, int reps, int order);
        Task<List<Guid>> GetSetIdsFromRoutineId(Guid routineId);
        Task DeleteSetsFromRoutineIdAsync(Guid routineId, List<Guid> setIds);
        Task<List<Routine>> GetRoutinesAsync(Guid userId);
        Task<MuscleArea> GetRoutineMuscleAreas(Guid routineId);
        Task<Set?> GetSetByExerciseIdAsync(Guid userId, Guid exerciseId);
        Task<List<MaxSet>> GetExerciseLeaderboardAsync(Guid exerciseId);

        Task<RoutineTemplate?> GetRoutineTemplateAsync(Guid userId, string name);
        Task<RoutineTemplate> GetRoutineTemplateAsync(Guid userId, Guid id);
        Task<RoutineTemplate> AddRoutineTemplateAsync(Guid id, Guid userId, string name);
        Task AddRoutineTemplateSetAsync(Guid routineTemplateId, Guid exerciseId);
        Task<List<RoutineTemplate>> GetRoutineTemplatesAsync(Guid userId);
        Task<List<Exercise>> GetRoutineTemplateSetsAsync(Guid userId, Guid id);
        Task UpdateRoutineTemplateNameAsync(Guid userId, Guid id, string name);
        Task DeleteRoutineTemplateSetsAsync(Guid id);
        Task DeleteRoutineTemplateAsync(Guid userId, Guid id);

        Task<int> GetWeeksWorkoutsCountAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<int> GetMonthsWorkoutsCountAsync(Guid userId, DateTime yearMonth);

        Task<Exercise> AddExerciseAsync(Exercise exercise);
        Task<ExerciseMuscle> AddExerciseMuscleAsync(Guid exerciseId, MuscleGroup muscle);
        Task<List<Routine>> GetRecentWorkoutsAsync();
    }
}
