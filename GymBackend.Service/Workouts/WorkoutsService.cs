using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;
using System.Reflection.Metadata.Ecma335;

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

        public async Task<RoutineSet?> GetRoutineAsync(Guid userId)
        {
            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date);

            if (routine == null) return null;

            var setsList = await storage.GetSetsByRoutineIdAsync(routine.Id);

            return new RoutineSet(routine.Id, setsList);
        }

        public async Task<RoutineSet> AddRoutineAsync(Guid userId, List<ExerciseSet> exerciseList)
        {
            if (exerciseList.Count == 0) throw new Exception("No exercises to add");

            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date) ?? await storage.AddRoutineAsync(Guid.NewGuid(), userId, DateTime.Now.Date);

            var exerciseSet = new List<Set>();

            await storage.DeleteSetsFromRoutineAsync(routine.Id);

            foreach (var exercise in exerciseList)
            {
                exerciseSet = await storage.AddExercisesAsync(Guid.NewGuid(), routine.Id, Guid.Parse(exercise.ExerciseId), new SetUpdate(exercise.Weight, exercise.Sets, exercise.Reps, exercise.Order));
            }

            if (exerciseList.Count == 0) throw new Exception("Error adding exercises to routine");

            return new RoutineSet(routine.Id, exerciseSet);
        }

        //public async Task<RoutineSet> UpdateRoutineAsync(string id, List<SetUpdate> setList)
        //{
        //    if (setList.Count == 0) throw new Exception("No sets to update");
        //    var routine = await storage.GetRoutineAsync(Guid.Parse(id)) ?? throw new Exception("Routine can't be found");

        //    var updatedSetList = new List<Set>();

        //    foreach (var exercise in setList)
        //    {
        //        updatedSetList = await storage.UpdateSetsForRoutineAsync(routine.Id, exercise);
        //    }

        //    return new RoutineSet(routine.Id, updatedSetList);
        //}

        public async Task DeleteSetFromRoutineAsync(Guid userId, string exerciseId)
        {
            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date) ?? null;
            await storage.DeleteSetFromRoutineAsync(routine.Id, Guid.Parse(exerciseId));

            var sets = await storage.GetSetsByRoutineIdAsync(routine.Id);
            if (sets.Count == 0) await storage.DeleteRoutineAsync(userId, routine.Id);
        }

        public async Task<Dictionary<Guid, int>> UpdateSetOrderAsync(Dictionary<Guid, int> setOrder)
        {
            try
            {
                return await storage.UpdateSetOrderAsync(setOrder);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot update set order", ex);
            }
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
    }
}
