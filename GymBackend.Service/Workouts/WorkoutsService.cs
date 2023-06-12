using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.User;
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

        public async Task<RoutineSet?> GetRoutineAsync(string userId)
        {
            var routine = await storage.GetRoutineAsync(Guid.Parse(userId), DateTime.Now.Date);

            if (routine == null) return null;

            var setsList = await storage.GetSetsByRoutineIdAsync(routine.Id);

            return new RoutineSet(routine.Id, setsList);
        }

        public async Task<RoutineSet> AddRoutineAsync(string userId, List<string> exerciseIds)
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

            return new RoutineSet(routine.Id, exerciseList);
        }

        public async Task<RoutineSet> UpdateRoutineAsync(string id, List<SetUpdate> setList)
        {
            if (setList.Count == 0) throw new Exception("No sets to update");
            var routine = await storage.GetRoutineAsync(Guid.Parse(id)) ?? throw new Exception("Routine can't be found");

            var updatedSetList = new List<Set>();

            foreach (var exercise in setList)
            {
                updatedSetList = await storage.UpdateSetsForRoutineAsync(routine.Id, exercise);
            }

            return new RoutineSet(routine.Id, updatedSetList);
        }
    }
}
