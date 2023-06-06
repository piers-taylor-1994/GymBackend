using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Service.Workouts
{
    public class WorkoutsService : IWorkoutsService
    {
        private readonly IWorkoutsStorage storage;

        public WorkoutsService(IWorkoutsStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }
        public Task<List<Exercises>> GetExercisesAsync()
        {
            return storage.GetAllExercisesAsync();
        }
    }
}
