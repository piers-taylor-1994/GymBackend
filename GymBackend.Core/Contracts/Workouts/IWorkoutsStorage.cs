using GymBackend.Core.Domains.User;
using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsStorage
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<List<Guid>> GetAllSearchExercisesAsync(MuscleGroup muscle);
        Task<Routine?> GetRoutineAsync(Guid userId, DateTime date);
        Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId);
        Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date);
        Task<List<Set>> AddExercisesAsync(Guid id, Guid routineId, ExerciseSet set);
        Task DeleteSetsFromRoutineAsync(Guid routineId);
        Task<List<Routine>> GetRoutinesAsync(Guid userId);
        Task<Set?> GetSetByExerciseIdAsync(Guid userId, Guid exerciseId);
        Task<List<MaxSet>> GetExerciseLeaderboardAsync(Guid exerciseId);
        Task<RoutineTemplate?> GetRoutineTemplateAsync(Guid userId, string name);
        Task<RoutineTemplate> GetRoutineTemplateAsync(Guid userId, Guid id);
        Task<RoutineTemplate> AddRoutineTemplateAsync(Guid id, Guid userId, string name);
        Task AddRoutineTemplateSetAsync(Guid routineTemplateId, Guid exerciseId);
        Task<List<RoutineTemplate>> GetRoutineTemplatesAsync(Guid userId);
        Task<List<Exercise>> GetRoutineTemplateSetsAsync(Guid userId, Guid id);
    }
}
