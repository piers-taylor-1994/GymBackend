using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;
using YOTApp.Storage;

namespace GymBackend.Storage.Workouts
{
    public class WorkoutsStorage : IWorkoutsStorage
    {
        private readonly IDatabase database;

        public WorkoutsStorage(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }
        public async Task<List<Exercises>> GetAllExercisesAsync()
        {
            var sql = "SELECT *, [MuscleGroupId] as MuscleGroup FROM [Workouts].[Exercises]";
            var exercises = await database.ExecuteQueryAsync<Exercises>(sql);
            return exercises.ToList();
        }

        public async Task<Guid> GetRoutineIdAsync(Guid userId, DateTime date)
        {
            var sql = "SELECT [Id] FROM [Workouts].[Routine] WHERE [UserId] = @userId";

            return await database.ExecuteQuerySingleAsync<Guid>(sql, new { userId });
        }

        public async Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId)
        {
            var sql = @"
SELECT [Name], [Weight], [Sets], [Reps]
FROM [Workouts].[Sets] s
INNER JOIN [Workouts].[Exercises] e
ON s.[ExerciseId] = e.[Id]
WHERE [RoutineId] = @routineId";
            var sets = await database.ExecuteQueryAsync<Set>(sql, new { routineId });
            return sets.ToList();
        }
    }
}
