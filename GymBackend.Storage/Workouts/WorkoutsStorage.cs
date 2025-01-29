using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;

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
            var sql = "SELECT *, [Id] as ExerciseId, [MuscleArea] FROM [Workouts].[Exercises]";
            var exercises = await database.ExecuteQueryAsync<Exercise>(sql);
            return exercises.ToList();
        }

        public async Task<List<Guid>> GetAllSearchExercisesAsync(MuscleGroup muscle)
        {
            var sql = @"
SELECT e.[Id]
FROM [Workouts].[Exercises] e
INNER JOIN [Workouts].[ExerciseMuscles] em on e.Id = em.ExerciseId
WHERE em.MuscleId = @muscle";
            var exercises = await database.ExecuteQueryAsync<Guid>(sql, new { muscle });
            return exercises.ToList();
        }

        public async Task<Routine?> GetRoutineAsync(Guid userId, DateTime date, string table)
        {
            var routines = await GetRoutinesAsync(userId, table);

            return routines.FirstOrDefault(r => r.Date.Date == date.Date);
        }

        public async Task<List<Set>> GetSetExerciseIdOrderByRoutineIdAsync(Guid routineId, string table)
        {
            var sql = $@"
SELECT s.[Id], s.[ExerciseId], e.[Name], s.[Order]
FROM [Workouts].[Exercises] e
INNER JOIN [Workouts].[{table}Sets] s
ON e.[Id] = s.[ExerciseId]
WHERE s.[RoutineId] = @routineId
ORDER BY s.[Order]";
            var exerciseSets = await database.ExecuteQueryAsync<Set>(sql, new { routineId });
            return exerciseSets.ToList();
        }

        public async Task<List<SetArray>> GetSetsArrayBySetId(Guid setId, string table)
        {
            var sql = $@"
SELECT sa.*
FROM [Workouts].[{table}SetsArray] sa
INNER JOIN [Workouts].[{table}Sets] s
ON sa.[SetId] = s.[Id]
WHERE sa.[SetId] = @setId
ORDER BY sa.[Order]";
            var sets = await database.ExecuteQueryAsync<SetArray>(sql, new { setId });
            return sets.ToList();
        }

        public async Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date, string table)
        {
            var sqlCreate = $@"
INSERT INTO [Workouts].[{table}Routine] ([Id], [UserId], [Date])
VALUES (
    @id,
    @userId,
    @date
)";
            await database.ExecuteAsync(sqlCreate, new { id, userId, date });

            var routine = await GetRoutineAsync(userId, date, table);

            return routine ?? throw new Exception("Create routine failed");
        }

        public async Task UpdateRoutineTimeAsync(Guid id, Guid userId, DateTime date, string table)
        {
            var sql = $@"
UPDATE [Workouts].[{table}Routine]
SET [Date] = @date
WHERE [Id] = @id AND [UserId] = @userId";
            await database.ExecuteAsync(sql, new { id, userId, date });
        }

        public async Task AddExercisesToSetAsync(Guid id, Guid routineId, Guid exerciseId, int order, string table)
        {
            var sqlCreate = $@"
INSERT INTO [Workouts].[{table}Sets] ([Id], [RoutineId], [ExerciseId], [Order])
VALUES (
    @id,
    @routineId,
    @exerciseId,
    @order
)";
            await database.ExecuteAsync(sqlCreate, new { id, routineId, exerciseId, order });
        }

        public async Task AddExerciseSetFromArrayAsync(Guid setId, float weight, int sets, int reps, int order, string table)
        {
            var sqlCreate = $@"
INSERT INTO [Workouts].[{table}SetsArray] ([SetId], [Weight], [Sets], [Reps], [Order])
VALUES (
    @setId,
    @weight,
    @sets,
    @reps,
    @order
)";
            await database.ExecuteAsync(sqlCreate, new { setId, weight, sets, reps, order });
        }

        public async Task<List<Guid>> GetSetIdsFromRoutineId(Guid routineId, string table)
        {
            var sqlGet = $@"
SELECT [Id]
FROM [Workouts].[{table}Sets]
WHERE [RoutineId] = @routineId";

            var sets = await database.ExecuteQueryAsync<Guid>(sqlGet, new { routineId });
            return sets.ToList();
        }

        public async Task DeleteSetsFromRoutineIdAsync(Guid routineId, List<Guid> setIds, string table)
        {
            foreach (var setId in setIds)
            {
                await database.ExecuteAsync($"DELETE FROM [Workouts].[{table}SetsArray] WHERE [SetId] = @setId", new { setId });
            }

            await database.ExecuteAsync($"DELETE FROM [Workouts].[{table}Sets] WHERE [RoutineId] = @routineId", new { routineId });
        }

        public async Task<MuscleArea> GetRoutineMuscleAreas(Guid routineId)
        {
            var sql = @"
  SELECT e.MuscleArea, COUNT(*)
  FROM [Workouts].[Exercises] e
  INNER JOIN [Workouts].[Sets] s on e.[Id] = s.[ExerciseId]
  WHERE s.[RoutineId] = @routineId
  GROUP BY e.MuscleArea
  ORDER BY COUNT(*) DESC";

            var muscles = await database.ExecuteQueryAsync<(MuscleArea muscle, int count)>(sql, new { routineId });
            return (muscles.Where(m => m.count == muscles.FirstOrDefault().count).Count() > 1) ? MuscleArea.Mixed : muscles.FirstOrDefault().muscle;
        }


        public async Task<List<Routine>> GetRoutinesAsync(Guid userId, string table)
        {
            var sql = $@"
SELECT DISTINCT [r].* 
FROM [Workouts].[{table}Routine] r
WHERE [UserId] = @userId
ORDER BY [Date] DESC";

            var routines = await database.ExecuteQueryAsync<Routine>(sql, new { userId });

            return routines.ToList();
        }

        public async Task<Set?> GetSetByExerciseIdAsync(Guid userId, Guid exerciseId)
        {
            var sql = @"
SELECT TOP(1) s.Id, s.RoutineId, s.ExerciseId, sa.Weight, sa.Sets, sa.Reps, r.Date
FROM Workouts.Sets s
INNER JOIN Workouts.Routine r on s.RoutineId = r.Id
INNER JOIN Workouts.SetsArray sa on s.Id = sa.SetId
WHERE s.ExerciseId = @exerciseId
AND r.UserId = @userId
ORDER BY r.Date desc";

            return await database.ExecuteQuerySingleAsync<Set>(sql, new { userId, exerciseId }) ?? null;
        }

        public async Task<List<MaxSet>> GetExerciseLeaderboardAsync(Guid exerciseId)
        {
            var sql = @"
SELECT TOP(10) u.Username, MAX(sa.Weight) as Weight
FROM [Workouts].[Exercises] e
INNER JOIN Workouts.[Sets] s ON e.Id = s.ExerciseId
INNER JOIN Workouts.[SetsArray] sa ON s.Id = sa.SetId
INNER JOIN Workouts.[Routine] r on s.RoutineId = r.Id
INNER JOIN Users.[Users] u on r.UserId = u.Id
WHERE e.Id = @exerciseId
GROUP BY u.Username
ORDER BY Weight desc";

            var maxSets = await database.ExecuteQueryAsync<MaxSet>(sql, new { exerciseId });
            return maxSets.ToList();
        }

        public async Task<RoutineTemplate?> GetRoutineTemplateAsync(Guid userId, string name)
        {
            var sql = "SELECT * FROM [Workouts].[RoutineTemplate] WHERE [UserId] = @userId AND [Name] = @name";

            return await database.ExecuteQuerySingleAsync<RoutineTemplate?>(sql, new { userId, name });
        }

        public async Task<RoutineTemplate> GetRoutineTemplateAsync(Guid userId, Guid id)
        {
            var sql = "SELECT * FROM [Workouts].[RoutineTemplate] WHERE [UserId] = @userId AND [Id] = @id";

            return await database.ExecuteQuerySingleAsync<RoutineTemplate>(sql, new { userId, id }) ?? throw new Exception("Cannot find routine template");
        }

        public async Task<RoutineTemplate> AddRoutineTemplateAsync(Guid id, Guid userId, string name)
        {
            var sql = @"
INSERT INTO [Workouts].[RoutineTemplate] ([Id], [UserId], [Name])
VALUES (
    @id,
    @userId,
    @name
)";

            await database.ExecuteAsync(sql, new { id, userId, name });
            return await GetRoutineTemplateAsync(userId, id);
        }

        public async Task AddRoutineTemplateSetAsync(Guid userId, Guid exerciseId)
        {
            var sql = @"
INSERT INTO [Workouts].[RoutineTemplateSets] ([RoutineTemplateId], [ExerciseId])
VALUES (
    @userId,
    @exerciseId
)";

            await database.ExecuteAsync(sql, new { userId, exerciseId });
        }

        public async Task<List<RoutineTemplate>> GetRoutineTemplatesAsync(Guid userId)
        {
            var sql = "SELECT * FROM [Workouts].[RoutineTemplate] WHERE [UserId] = @userId";

            var routines = await database.ExecuteQueryAsync<RoutineTemplate>(sql, new { userId });

            return routines.ToList();
        }

        public async Task<List<Exercise>> GetRoutineTemplateSetsAsync(Guid userId, Guid id)
        {
            var sql = @"
SELECT e.[Id] as ExerciseId, e.[MuscleArea] as MuscleGroup, [e].*
FROM [Workouts].[RoutineTemplateSets] s
INNER JOIN [Workouts].[RoutineTemplate] r on s.RoutineTemplateId = r.Id
INNER JOIN [Workouts].[Exercises] e on s.ExerciseId = e.Id
WHERE r.Id = @id
AND r.UserId = @userId
ORDER BY s.[Id]";

            var sets = await database.ExecuteQueryAsync<Exercise>(sql, new { userId, id });

            return sets.ToList();
        }

        public async Task UpdateRoutineTemplateNameAsync(Guid userId, Guid id, string name)
        {
            var sql = @"
UPDATE [Workouts].[RoutineTemplate]
SET [Name] = @name
WHERE [Id] = @id
AND [UserId] = @userId";

            await database.ExecuteAsync(sql, new { userId, id, name });
        }

        public async Task DeleteRoutineTemplateSetsAsync(Guid id)
        {
            var sql = "DELETE FROM [Workouts].[RoutineTemplateSets] WHERE [RoutineTemplateId] = @id";

            await database.ExecuteAsync(sql, new { id });
        }

        public async Task DeleteRoutineTemplateAsync(Guid userId, Guid id)
        {
            var sql = "DELETE FROM [Workouts].[RoutineTemplate] WHERE [Id] = @id AND [UserId] = @userId";

            await database.ExecuteAsync(sql, new { userId, id });
        }

        public async Task<int> GetWeeksWorkoutsCountAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var sql = @"
SELECT COUNT(*)
FROM [Workouts].[Routine]
WHERE UserId = @userId 
AND Date >= @startDate
AND Date <= @endDate";

            return await database.ExecuteQuerySingleAsync<int>(sql, new { userId, startDate, endDate });
        }

        public async Task<int> GetMonthsWorkoutsCountAsync(Guid userId, DateTime yearMonth)
        {
            var sql = @"
SELECT COUNT(*)
FROM [Workouts].[Routine]
WHERE UserId = @userId
AND Date >= @yearMonth
AND Date <= DATEADD(month, 1, @yearMonth)";

            return await database.ExecuteQuerySingleAsync<int>(sql, new { userId, yearMonth });
        }

        public async Task<Exercise> AddExerciseAsync(Exercise exercise)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[Exercises] ([Id], [MuscleArea], [Name])
VALUES (
    @ExerciseId,
    @MuscleArea,
    @Name
)";
            await database.ExecuteAsync(sqlCreate, exercise);

            var sqlGet = "SELECT Id AS ExerciseId, MuscleArea, Name FROM [Workouts].[Exercises] WHERE [Id] = @ExerciseId";

            return await database.ExecuteQuerySingleAsync<Exercise>(sqlGet, exercise) ?? throw new Exception("Create exercise failed");
        }

        public async Task<ExerciseMuscle> AddExerciseMuscleAsync(Guid exerciseId, MuscleGroup muscle)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[ExerciseMuscles] ([ExerciseId], [MuscleId])
VALUES (
    @exerciseId,
    @muscle
)";
            await database.ExecuteAsync(sqlCreate, new { exerciseId, muscle });

            var sqlGet = "SELECT * FROM [Workouts].[ExerciseMuscles] WHERE [ExerciseId] = @exerciseId AND [MuscleId] = @muscle";

            return await database.ExecuteQuerySingleAsync<ExerciseMuscle>(sqlGet, new { exerciseId, muscle }) ?? throw new Exception("Create exercise muscle failed");
        }

        public async Task<List<Routine>> GetRecentWorkoutsAsync()
        {
            var sql = @"
SELECT TOP 5 *
FROM [Workouts].[Routine] r
ORDER BY Date desc";
            var routines = await database.ExecuteQueryAsync<Routine>(sql);
            return routines.ToList();
        }

        public async Task DeleteRoutineDataAsync(Guid userId, DateTime date, string table)
        {
            var routines = await database.ExecuteQueryAsync<Routine>($"SELECT * FROM [Workouts].[{table}Routine] WHERE UserId = @userId", new { userId });
            var routine = routines.FirstOrDefault(g => g.Date.Date == date.Date);

            if (routine != null)
            {
                var setsIds = await database.ExecuteQueryAsync<Guid>($"SELECT [Id] FROM [Workouts].[{table}Sets] WHERE [RoutineId] = @Id", new { routine.Id });

                foreach (var setId in setsIds)
                {
                    await database.ExecuteAsync($"DELETE FROM [Workouts].[{table}SetsArray] WHERE [SetId] = @setId", new { setId });
                    await database.ExecuteAsync($"DELETE FROM [Workouts].[{table}Sets] WHERE [Id] = @setId", new { setId });
                }

                await database.ExecuteAsync($"DELETE FROM [Workouts].[{table}Routine] WHERE [Id] = @Id", new { routine.Id });
            }
        }
    }
}
