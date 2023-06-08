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

        public async Task<List<Exercise>> GetAllExercisesAsync()
        {
            var sql = "SELECT *, [MuscleGroupId] as MuscleGroup FROM [Workouts].[Exercises]";
            var exercises = await database.ExecuteQueryAsync<Exercise>(sql);
            return exercises.ToList();
        }

        public async Task<Routine?> GetRoutineAsync(Guid userId, DateTime date)
        {
            var sql = "SELECT * FROM [Workouts].[Routine] WHERE [UserId] = @userId AND [Date] = @date";

            return await database.ExecuteQuerySingleAsync<Routine>(sql, new { userId, date }) ?? null;
        }

        public async Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId)
        {
            var sql = @"
SELECT e.*, e.[MuscleGroupId] as MuscleGroup, s.[Weight], s.[Sets], s.[Reps]
FROM [Workouts].[Exercises] e
INNER JOIN [Workouts].[Sets] s
ON e.[Id] = s.[ExerciseId]
WHERE s.[RoutineId] = @routineId";
            var sets = await database.ExecuteQueryAsync<Set>(sql, new { routineId });
            return sets.ToList();
        }

        public async Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[Routine] ([Id], [UserId], [Date])
VALUES (
    @id,
    @userId,
    @date
)";
            await database.ExecuteAsync(sqlCreate, new { id, userId, date });

            var routine = await GetRoutineAsync(userId, date);

            return routine ?? throw new Exception("Create routine failed");
        }

        public async Task<List<Set>> AddExercisesAsync(Guid id, Guid routineId, Guid exerciseId)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[Sets] ([Id], [RoutineId], [ExerciseId])
VALUES (
    @id,
    @routineId,
    @exerciseId
)";
            await database.ExecuteAsync(sqlCreate, new { id, routineId, exerciseId });

            return await GetSetsByRoutineIdAsync(routineId);
        }

        public async Task DeleteSetsForRoutineAsync(Guid id)
        {
            var sql = "DELETE FROM [Workouts].[Sets] WHERE [RoutineId] = @id";

            await database.ExecuteAsync(sql, new { id });
        }
    }
}
