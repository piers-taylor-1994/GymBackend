using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsStorage
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<Guid> GetRoutineIdAsync(Guid userId, DateTime date);
        Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId);
    }
}
