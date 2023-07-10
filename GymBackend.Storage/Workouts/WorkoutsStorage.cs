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
            var sql = "SELECT *, [Id] as ExerciseId, [MuscleGroupId] as MuscleGroup FROM [Workouts].[Exercises]";
            var exercises = await database.ExecuteQueryAsync<Exercise>(sql);
            return exercises.ToList();
        }

        public async Task<List<Guid>> GetAllSearchExercisesAsync(MuscleGroup muscle)
        {
            var sql = @"
SELECT e.[Id]
FROM [Workouts].[Exercises] e
INNER JOIN [Workouts].[ExerciseMuscles] em on e.Id = em.ExerciseId
INNER JOIN [Workouts].[MuscleGroups] m on em.MuscleId = m.Id
WHERE em.MuscleId = @muscle";
            var exercises = await database.ExecuteQueryAsync<Guid>(sql, new { muscle });
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
SELECT s.[Id], e.[Id] as ExerciseId, e.[MuscleGroupId] as MuscleGroup, e.[Name], e.[Description], s.[Weight], s.[Sets], s.[Reps], s.[Order]
FROM [Workouts].[Exercises] e
INNER JOIN [Workouts].[Sets] s
ON e.[Id] = s.[ExerciseId]
WHERE s.[RoutineId] = @routineId
ORDER BY s.[Order]";
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

        public async Task<List<Set>> AddExercisesAsync(Guid id, Guid routineId, ExerciseSet set)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[Sets] ([Id], [RoutineId], [ExerciseId], [Weight], [Sets], [Reps], [Order])
VALUES (
    @id,
    @routineId,
    @ExerciseId,
    @Weight,
    @Sets,
    @Reps,
    @Order
)";
            await database.ExecuteAsync(sqlCreate, new { id, routineId, set.ExerciseId, set.Weight, set.Sets, set.Reps, set.Order });

            return await GetSetsByRoutineIdAsync(routineId);
        }

        public async Task DeleteSetsFromRoutineAsync(Guid routineId)
        {
            var sql = "DELETE FROM [Workouts].[Sets] WHERE [RoutineId] = @routineId";

            await database.ExecuteAsync(sql, new { routineId });
        }


        public async Task<List<Routine>> GetRoutinesAsync(Guid userId)
        {
            var sql = @"
SELECT DISTINCT [r].* 
FROM [Workouts].[Routine] r
INNER JOIN [Workouts].[Sets] s ON [r].[Id] = [s].[RoutineId] 
WHERE [UserId] = @userId
AND [s].[Weight] IS NOT NULL 
AND [s].[Sets] IS NOT NULL 
AND [s].[Reps] IS NOT NULL 
ORDER BY [Date] DESC";

            var routines = await database.ExecuteQueryAsync<Routine>(sql, new { userId });

            return routines.ToList();
        }

        public async Task<Set?> GetSetByExerciseIdAsync(Guid userId, Guid exerciseId)
        {
            var sql = @"
SELECT TOP(1) s.*, r.Date
FROM Workouts.Sets s
INNER JOIN Workouts.Routine r on s.RoutineId = r.Id
WHERE s.ExerciseId = @exerciseId
AND r.UserId = @userId
AND s.Weight IS NOT NULL
AND s.Sets IS NOT NULL
AND s.Reps IS NOT NULL
ORDER BY r.Date desc";

            return await database.ExecuteQuerySingleAsync<Set>(sql, new { userId, exerciseId }) ?? null;
        }

        public async Task<List<MaxSet>> GetExerciseLeaderboardAsync(Guid exerciseId)
        {
            var sql = @"
SELECT TOP(10) u.Username, MAX(s.Weight) as Weight
FROM Workouts.Sets s
INNER JOIN Workouts.Exercises e on s.ExerciseId = e.Id
INNER JOIN Workouts.Routine r on s.RoutineId = r.Id
INNER JOIN Users.Users u on r.UserId = u.Id
WHERE e.Id = @exerciseId
GROUP BY u.Username
ORDER BY Weight DESC";

            var maxSets = await database.ExecuteQueryAsync<MaxSet>(sql, new { exerciseId });
            return maxSets.ToList();
        }
    }
}
