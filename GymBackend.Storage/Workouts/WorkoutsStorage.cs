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

        public async Task<Routine?> GetRoutineAsync(Guid userId, DateTime date)
        {
            var sql = "SELECT * FROM [Workouts].[Routine] WHERE [UserId] = @userId AND [Date] = @date";

            return await database.ExecuteQuerySingleAsync<Routine>(sql, new { userId, date }) ?? null;
        }

        public async Task<List<Set>> GetSetExerciseIdOrderByRoutineIdAsync(Guid routineId)
        {
            var sql = @"
SELECT s.[Id], s.[ExerciseId], e.[Name], e.[Type], s.[Order]
FROM [Workouts].[Exercises] e
INNER JOIN [Workouts].[Sets] s
ON e.[Id] = s.[ExerciseId]
WHERE s.[RoutineId] = @routineId
ORDER BY s.[Order]";
            var exerciseSets = await database.ExecuteQueryAsync<Set>(sql, new { routineId });
            return exerciseSets.ToList();
        }

        public async Task<List<SetArray>> GetSetsArrayBySetId(Guid setId)
        {
            var sql = @"
SELECT sa.*
FROM [Workouts].[SetsArray] sa
INNER JOIN [Workouts].[Sets] s
ON sa.[SetId] = s.[Id]
WHERE sa.[SetId] = @setId
ORDER BY sa.[Order]";
            var sets = await database.ExecuteQueryAsync<SetArray>(sql, new { setId });
            return sets.ToList();
        }

        public async Task<List<SetArray>> GetSetsTimedArrayBySetId(Guid setId)
        {
            var sql = @"
SELECT sta.[Id], sta.[SetId], sta.[Weight], sta.[Sets], sta.[Seconds] as Reps, sta.[Order]
FROM [Workouts].[SetsTimedArray] sta
INNER JOIN [Workouts].[Sets] s
ON sta.[SetId] = s.[Id]
WHERE sta.[SetId] = @setId
ORDER BY sta.[Order]";
            var sets = await database.ExecuteQueryAsync<SetArray>(sql, new { setId });
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

        public async Task AddExercisesToSetAsync(Guid id, Guid routineId, Guid exerciseId, int order)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[Sets] ([Id], [RoutineId], [ExerciseId], [Order])
VALUES (
    @id,
    @routineId,
    @exerciseId,
    @order
)";
            await database.ExecuteAsync(sqlCreate, new { id, routineId, exerciseId, order });
        }

        public async Task AddExerciseSetFromArrayAsync(Guid setId, float weight, int sets, int reps, int order)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[SetsArray] ([SetId], [Weight], [Sets], [Reps], [Order])
VALUES (
    @setId,
    @weight,
    @sets,
    @reps,
    @order
)";
            await database.ExecuteAsync(sqlCreate, new { setId, weight, sets, reps, order });
        }

        public async Task AddExerciseTimedSetFromArrayAsync(Guid setId, float weight, int sets, int reps, int order)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[SetsArray] ([SetId], [Weight], [Sets], [Seconds], [Order])
VALUES (
    @setId,
    @weight,
    @sets,
    @reps,
    @order
)";
            await database.ExecuteAsync(sqlCreate, new { setId, weight, sets, reps, order });
        }

        public async Task<List<Guid>> GetSetIdsFromRoutineId(Guid routineId)
        {
            var sqlGet = @"
SELECT [Id]
FROM [Workouts].[Sets]
WHERE [RoutineId] = @routineId";

            var sets = await database.ExecuteQueryAsync<Guid>(sqlGet, new { routineId });
            return sets.ToList();
        }

        public async Task DeleteSetArrayFromRoutineIdAsync(Guid setId)
        {
            var sql = @"
DELETE FROM [Workouts].[SetsArray] WHERE [SetId] = @setId
DELETE FROM [Workouts].[SetsTimedArray] WHERE [SetId] = @setId";

            await database.ExecuteAsync(sql, new { setId });
        }

        public async Task DeleteSetsFromRoutineIdAsync(Guid routineId)
        {
            var sql = "DELETE FROM [Workouts].[Sets] WHERE [RoutineId] = @routineId";

            await database.ExecuteAsync(sql, new { routineId });
        }

        public async Task<MuscleArea> GetRoutineMuscleAreas(Guid routineId)
        {
            var sql = @"
  SELECT TOP(1) e.MuscleArea
  FROM [Workouts].[Exercises] e
  INNER JOIN [Workouts].[Sets] s on e.[Id] = s.[ExerciseId]
  WHERE s.[RoutineId] = @routineId
  GROUP BY e.MuscleArea
  ORDER BY COUNT(*) DESC";

            return await database.ExecuteQuerySingleAsync<MuscleArea>(sql, new { routineId });
        }


        public async Task<List<Routine>> GetRoutinesAsync(Guid userId)
        {
            var sql = @"
SELECT DISTINCT [r].* 
FROM [Workouts].[Routine] r
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

        public async Task<Set?> GetTimedSetByExerciseIdAsync(Guid userId, Guid exerciseId)
        {
            var sql = @"
SELECT TOP(1) s.Id, s.RoutineId, s.ExerciseId, sta.Weight, sta.Sets, sta.Seconds as Reps, r.Date
FROM Workouts.Sets s
INNER JOIN Workouts.Routine r on s.RoutineId = r.Id
INNER JOIN Workouts.SetsTimedArray staa on s.Id = sta.SetId
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

        public async Task<int> GetWeeksWorkoutsCountAsync(Guid userId, DateTime from, DateTime to)
        {
            var sql = @"
SELECT COUNT(*)
FROM [Workouts].[Routine]
WHERE UserId = @userId
AND Date <= @from 
AND Date >= @to";

            return await database.ExecuteQuerySingleAsync<int>(sql, new { userId, from, to });
        }

        public async Task<int> GetMonthsWorkoutsCountAsync(Guid userId, DateTime yearMonth)
        {
            var sql = @"
SELECT COUNT(*)
FROM [Workouts].[Routine]
WHERE UserId = @userId
AND Date >= @yearMonth
AND Date < DATEADD(month, 1, @yearMonth)";

            return await database.ExecuteQuerySingleAsync<int>(sql, new { userId, yearMonth });
        }

        public async Task<Exercise> AddExerciseAsync(Exercise exercise)
        {
            var sqlCreate = @"
INSERT INTO [Workouts].[Exercises] ([Id], [MuscleArea], [Name], [Type])
VALUES (
    @ExerciseId,
    @MuscleArea,
    @Name,
    @Type
)";
            await database.ExecuteAsync(sqlCreate, exercise);

            var sqlGet = "SELECT Id AS ExerciseId, MuscleArea, Name, Type FROM [Workouts].[Exercises] WHERE [Id] = @ExerciseId";

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
    }
}
