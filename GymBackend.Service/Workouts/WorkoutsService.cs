using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;
using System;

namespace GymBackend.Service.Workouts
{
    public class WorkoutsService : IWorkoutsService
    {
        private readonly IWorkoutsStorage storage;

        public WorkoutsService(IWorkoutsStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }
        public Task<List<Exercise>> GetExercisesAsync()
        {
            return storage.GetAllExercisesAsync();
        }

        public async Task<Routine> GetRoutineAsync(string userId)
        {
            var routineId = await storage.GetRoutineIdAsync(Guid.Parse(userId), DateTime.Now.Date);
            var setsList = await storage.GetSetsByRoutineIdAsync(routineId);

            return new Routine(setsList);
        }
    }
}
