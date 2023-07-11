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

        public async Task<List<Exercise>> GetExercisesAsync()
        {
            return await storage.GetAllExercisesAsync();
        }

        public async Task<List<Guid>> SearchExercisesAsync(MuscleGroup muscle)
        {
            return await storage.GetAllSearchExercisesAsync(muscle);
        }

        public async Task<RoutineSet?> GetRoutineAsync(Guid userId)
        {
            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date);

            if (routine == null) return null;

            var setsList = await storage.GetSetsByRoutineIdAsync(routine.Id);

            return new RoutineSet(routine.Id, setsList);
        }

        public async Task<RoutineSet> AddRoutineAsync(Guid userId, List<ExerciseSet> sets)
        {
            if (sets.Count == 0) throw new Exception("No exercises to add");

            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date);

            if (routine == null)
            {
                routine = await storage.AddRoutineAsync(Guid.NewGuid(), userId, DateTime.Now.Date);
            }
            else
            {
                await storage.DeleteSetsFromRoutineAsync(routine.Id);
            }

            var setList = new List<Set>();

            foreach (var set in sets)
            {
                setList = await storage.AddExercisesAsync(Guid.NewGuid(), routine.Id, set);
            }

            return new RoutineSet(routine.Id, setList);
        }

        public Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId)
        {
            return storage.GetRoutinesAsync(userId);
        }

        public async Task<RoutineSet> GetRoutineHistoryAsync(string id)
        {
            var setsList = await storage.GetSetsByRoutineIdAsync(Guid.Parse(id));

            return new RoutineSet(Guid.Parse(id), setsList);
        }

        public async Task<List<Set>> GetLastSetForExercisesAsync(Guid userId, List<string> exerciseIds)
        {
            var setList = new List<Set>();

            foreach (var exerciseId in exerciseIds)
            {
                var set = await storage.GetSetByExerciseIdAsync(userId, Guid.Parse(exerciseId));
                if (set != null) setList.Add(set);
            }

            return setList;
        }

        public async Task<List<MaxSet>> GetExerciseLeaderboardAsync(string exerciseId)
        {
            return await storage.GetExerciseLeaderboardAsync(Guid.Parse(exerciseId));
        }
    }
}
