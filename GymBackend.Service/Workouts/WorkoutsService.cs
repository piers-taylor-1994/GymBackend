﻿using GymBackend.Core.Contracts.Workouts;
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

        public async Task<RoutineSet> AddRoutineAsync(Guid userId, List<string> exerciseIds)
        {
            if (exerciseIds.Count == 0) throw new Exception("No exercises to add");

            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date) ?? await storage.AddRoutineAsync(Guid.NewGuid(), userId, DateTime.Now.Date);

            var setList = await storage.GetSetsByRoutineIdAsync(routine.Id);

            foreach (var set in setList)
            {
                if (!exerciseIds.Any(e => Guid.Parse(e) == set.ExerciseId)) 
                {
                    setList = await storage.DeleteSetFromRoutineAsync(routine.Id, set.Id);
                }
            }

            foreach (var id in exerciseIds)
            {
                if (!setList.Any(s => s.ExerciseId == Guid.Parse(id))) setList = await storage.AddExercisesAsync(Guid.NewGuid(), routine.Id, Guid.Parse(id), setList.Count);
            }

            if (setList.Count == 0) throw new Exception("Error adding exercises to routine");

            return new RoutineSet(routine.Id, setList);
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

        public async Task DeleteSetFromRoutineAsync(Guid userId, string id)
        {
            var routine = await storage.GetRoutineAsync(userId, DateTime.Now.Date) ?? throw new Exception("Cannot find routine");
            await storage.DeleteSetFromRoutineAsync(routine.Id, Guid.Parse(id));

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
    }
}
