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

        public Task<List<Exercise>> GetExercisesAsync()
        {
            return storage.GetAllExercisesAsync();
        }

        public async Task<RoutineView> GetRoutineAsync(string userId)
        {
            var routine = await storage.GetRoutineAsync(Guid.Parse(userId), DateTime.Now.Date) ?? throw new Exception("Routine doesn't exist");

            var setsList = await storage.GetSetsByRoutineIdAsync(routine.Id);

            return new RoutineView(routine.Id, setsList);
        }

        public async Task<RoutineView> AddRoutineAsync(string userId, List<string> exerciseIds)
        {
            if (exerciseIds.Count == 0) throw new Exception("No exercises to add");

            var routine = await storage.GetRoutineAsync(Guid.Parse(userId), DateTime.Now.Date) ?? await storage.AddRoutineAsync(Guid.NewGuid(), Guid.Parse(userId), DateTime.Now.Date);

            var exerciseList = await storage.GetSetsByRoutineIdAsync(routine.Id);
            if (exerciseList.Count != 0) await storage.DeleteSetsForRoutineAsync(routine.Id);

            foreach (var exerciseId in exerciseIds) 
            {
               exerciseList = await storage.AddExercisesAsync(Guid.NewGuid(), routine.Id, Guid.Parse(exerciseId));
            }

            if (exerciseList.Count == 0) throw new Exception("Error adding exercises to routine");

            return new RoutineView(routine.Id, exerciseList);
        }
    }
}
