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

        public async Task<RoutineSet?> GetRoutineAsync(Guid userId, int submissionType)
        {
            var table = TableGenerator((SubmissionType)submissionType);
            var routine = await storage.GetRoutineAsync(userId, DateTime.Now, table);

            if (routine == null) return null;

            var exerciseOrdersList = await storage.GetSetExerciseIdOrderByRoutineIdAsync(routine.Id);

            var list = new List<ExerciseSets>();

            foreach (var setOrder in exerciseOrdersList)
            {
                var sets = await storage.GetSetsArrayBySetId(setOrder.Id);

                list.Add(new ExerciseSets(setOrder.Id, setOrder.ExerciseId, setOrder.Name, setOrder.Order, sets));
            }

            return new RoutineSet(routine.Id, list);

        }

        //TODO: Add a controller method to add a ghost routine to the GhostRoutine/GhostSets/GhostSetsArray tables
        //Then a user can add a ghost routine if they forgot to click the submit button
        //It'll hijack all of the existing addroutine stuff
        public async Task<Guid> AddRoutineAsync(Guid userId, List<ExerciseSets> exerciseSets, int submissionType)
        {
            if (exerciseSets.Count == 0) throw new Exception("No exercises to add");
            var table = TableGenerator((SubmissionType)submissionType);

            var routines = await storage.GetRoutinesAsync(userId, table);
            var routine = routines.FirstOrDefault(r => r.Date.Date == DateTime.Now.Date);

            if (routine == null) routine = await storage.AddRoutineAsync(Guid.NewGuid(), userId, DateTime.Now, table);
            else
            {
                await storage.UpdateRoutineTimeAsync(routine.Id, userId, DateTime.Now, table);

                var setIdList = await storage.GetSetIdsFromRoutineId(routine.Id, table);

                await storage.DeleteSetsFromRoutineIdAsync(routine.Id, setIdList, table);
            }

            foreach (var exercise in exerciseSets)
            {
                var setId = Guid.NewGuid();
                await storage.AddExercisesToSetAsync(setId, routine.Id, exercise.ExerciseId, exercise.Order, table);

                foreach (var set in exercise.ExerciseArray)
                {
                    if (set.Weight is not null and >= 0  && set.Sets is not null and > 0 && set.Reps is not null and > 0) await storage.AddExerciseSetFromArrayAsync(setId, set.Weight.Value, set.Sets.Value, set.Reps.Value, set.Order, table);
                }
            }

            return routine.Id;
        }

        public async Task<List<Routine>> GetRoutinesHistoryAsync(Guid userId)
        {
            var routines = await storage.GetRoutinesAsync(userId, "");

            List<Routine> routineMuscleAreas = new();

            foreach (var routine in routines)
            {
                var muscleArea = await storage.GetRoutineMuscleAreas(routine.Id);

                routineMuscleAreas.Add(new Routine { Id = routine.Id, Date = routine.Date, MuscleArea = muscleArea });
            }

            return routineMuscleAreas;
        }

        public async Task<RoutineSet> GetRoutineHistoryAsync(string id)
        {
            var exerciseOrdersList = await storage.GetSetExerciseIdOrderByRoutineIdAsync(Guid.Parse(id));

            var list = new List<ExerciseSets>();

            foreach (var setOrder in exerciseOrdersList)
            {
                var sets = await storage.GetSetsArrayBySetId(setOrder.Id);

                list.Add(new ExerciseSets(setOrder.Id, setOrder.ExerciseId, setOrder.Name, setOrder.Order, sets));
            }

            return new RoutineSet(Guid.Parse(id), list);
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

        public async Task<List<RoutineTemplate>> GetRoutineTemplatesAsync(Guid userId)
        {
            return await storage.GetRoutineTemplatesAsync(userId);
        }

        public async Task<List<Exercise>> GetRoutineTemplateSetsAsync(Guid userId, string id)
        {
            return await storage.GetRoutineTemplateSetsAsync(userId, Guid.Parse(id));
        }

        public async Task<RoutineTemplate> AddRoutineTemplateAsync(Guid userId, string name, List<string> exerciseIds)
        {
            var routineTemplate = await storage.GetRoutineTemplateAsync(userId, name);

            if (routineTemplate != null) throw new Exception("Routine name already exists");

            routineTemplate = await storage.AddRoutineTemplateAsync(Guid.NewGuid(), userId, name);

            foreach (var exerciseId in exerciseIds)
            {
                await storage.AddRoutineTemplateSetAsync(routineTemplate.Id, Guid.Parse(exerciseId));
            }

            return await storage.GetRoutineTemplateAsync(userId, routineTemplate.Id);
        }

        public async Task<List<RoutineTemplate>> UpdateRoutineTemplateAsync(Guid userId, string id, string name, List<string> exerciseIds)
        {
            await storage.UpdateRoutineTemplateNameAsync(userId, Guid.Parse(id), name);
            await storage.DeleteRoutineTemplateSetsAsync(Guid.Parse(id));

            foreach (var exerciseId in exerciseIds)
            {
                await storage.AddRoutineTemplateSetAsync(Guid.Parse(id), Guid.Parse(exerciseId));
            }

            return await storage.GetRoutineTemplatesAsync(userId);
        }

        public async Task<List<RoutineTemplate>> DeleteRoutineTemplateAsync(Guid userId, string id)
        {
            await storage.DeleteRoutineTemplateSetsAsync(Guid.Parse(id));
            await storage.DeleteRoutineTemplateAsync(userId, Guid.Parse(id));

            return await storage.GetRoutineTemplatesAsync(userId);
        }

        public async Task<WorkoutsCount> GetWorkoutsCountAsync(Guid userId)
        {
            DateTime originalDate = DateTime.Now;
            int i = 0;
            while (originalDate.DayOfWeek != DayOfWeek.Monday)
            {
                originalDate = originalDate.AddDays(-1);
                i++;
            };

            var thisWeeksCount = await storage.GetWeeksWorkoutsCountAsync(userId, DateTime.Now.AddDays(-i).Date, DateTime.Now);
            var lastWeeksCount = await storage.GetWeeksWorkoutsCountAsync(userId, DateTime.Now.Date.AddDays(-(i + 7)), DateTime.Now.Date.AddDays(-(i + 1)).AddHours(23).AddMinutes(59).AddSeconds(59));
            var thisMonthsCount = await storage.GetMonthsWorkoutsCountAsync(userId, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
            var lastMonthsCount = await storage.GetMonthsWorkoutsCountAsync(userId, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01).AddMonths(-1));
            return new WorkoutsCount(thisWeeksCount, lastWeeksCount, thisMonthsCount, lastMonthsCount);
        }

        public async Task<Exercise> AddExerciseAsync(string name, List<MuscleGroup> muscles)
        {
            Guid exerciseId = Guid.NewGuid();

            Dictionary<MuscleArea, int> muscleAreaCount = new()
            {
                { MuscleArea.Upper, muscles.Where(m => (int)m <= 5).Count() },
                { MuscleArea.Core, muscles.Where(m => (int)m == 6).Count() },
                { MuscleArea.Lower, muscles.Where(m => (int)m > 6).Count() },

            };

            var exercise = await storage.AddExerciseAsync(new Exercise() { ExerciseId = exerciseId, MuscleArea = muscleAreaCount.MaxBy(t => t.Value).Key, Name = name });
            foreach (var muscle in muscles) { await storage.AddExerciseMuscleAsync(exerciseId, muscle); }

            return exercise;
        }

        public async Task<List<Routine>> GetMostRecentWorkoutsAsync()
        {
            var routines = await storage.GetRecentWorkoutsAsync();

            foreach (var routine in routines)
            {
                routine.MuscleArea = await storage.GetRoutineMuscleAreas(routine.Id);
            }

            return routines;
        }

        private static string TableGenerator(SubmissionType submissionType)
        {
            return submissionType == SubmissionType.Ghost ? "Ghost" : string.Empty;
        }
    }
}
