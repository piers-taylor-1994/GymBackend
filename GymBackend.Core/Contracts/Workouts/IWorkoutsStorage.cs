using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsStorage
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<Routine?> GetRoutineAsync(Guid userId, DateTime date);
        Task<Routine?> GetRoutineAsync(Guid id);
        Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId);
        Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date);
        Task<List<Set>> AddExercisesAsync(Guid id, Guid routineId, Guid exerciseId);
        Task DeleteSetFromRoutineAsync(Guid routineId, Guid setId);
        Task DeleteSetsFromRoutineAsync(Guid routineId);
        Task<List<Set>> UpdateSetsForRoutineAsync(Guid routineId, SetUpdate set);
        Task<List<Routine>> GetRoutinesAsync(Guid userId);
        Task DeleteRoutineAsync(Guid userId, Guid routineId);
    }
}
