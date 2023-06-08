using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsStorage
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<Routine?> GetRoutineAsync(Guid userId, DateTime date);
        Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId);
        Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date);
        Task<List<Set>> AddExercisesAsync(Guid id, Guid routineId, Guid exerciseId);
        Task DeleteSetsForRoutineAsync(Guid id);
    }
}
