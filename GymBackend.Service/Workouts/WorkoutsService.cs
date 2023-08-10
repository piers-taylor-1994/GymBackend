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
                setList = await storage.AddExercisesToSetAsync(Guid.NewGuid(), routine.Id, set);
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

        public async Task<WorkoutsCount> GetWorkoutsCountAsync(Guid userId)
        {
            DateTime originalDate = DateTime.Now;
            int i = 0;
            while (originalDate.DayOfWeek != DayOfWeek.Monday)
            {
                originalDate = originalDate.AddDays(-1);
                i++;
            };

            var thisWeeksCount = await storage.GetWeeksWorkoutsCountAsync(userId, DateTime.Now.Date, DateTime.Now.AddDays(-i).Date);
            var lastWeeksCount = await storage.GetWeeksWorkoutsCountAsync(userId, DateTime.Now.Date.AddDays(-(i + 1)), DateTime.Now.Date.AddDays(-(i + 7)));
            var thisMonthsCount = await storage.GetMonthsWorkoutsCountAsync(userId, new DateTime(2023, DateTime.Now.Month, 01));
            var lastMonthsCount = await storage.GetMonthsWorkoutsCountAsync(userId, new DateTime(2023, DateTime.Now.Month, 01).AddMonths(-1));
            return new WorkoutsCount(thisWeeksCount, lastWeeksCount, thisMonthsCount, lastMonthsCount);
        }

        public async Task<Exercise> AddExerciseAsync(string name, List<MuscleGroup> muscles)
        {
            Guid exerciseId = Guid.NewGuid();

            var exercise = await storage.AddExerciseAsync(new Exercise() { ExerciseId = exerciseId, MuscleGroup = 0, Name = name, Description = "Do " + name });
            foreach (var muscle in muscles) { await storage.AddExerciseMuscleAsync(exerciseId, muscle); }

            return exercise;
        }
    }
}
