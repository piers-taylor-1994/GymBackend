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

        // Beginnings of making history coloured and labelled by muscle
        // Probs need to remove foreign key in [Exercises] as it is reference MuscleGroups (biceps etc) and this is reference MuscleAreas (upper/lower)
//        public async Task<int> GetMostCommonMuscleGroupFromSetsAsync()
//        {
//            var sql = @"
//SELECT TOP 1 e.[MuscleGroupId]
//FROM [Workouts].[Sets] s
//INNER JOIN [Workouts].[Exercises] e on s.[ExerciseId] = e.[Id]
//WHERE RoutineId = 'F5F26AF7-DE2F-42C1-8EDA-4ABDEEF5CD4E'
//GROUP BY e.[MuscleGroupId]
//ORDER BY COUNT(*) DESC";
//        }

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
SELECT e.[Id] as ExerciseId, e.[MuscleGroupId] as MuscleGroup, [e].*
FROM [Workouts].[RoutineTemplateSets] s
INNER JOIN [Workouts].[RoutineTemplate] r on s.RoutineTemplateId = r.Id
INNER JOIN [Workouts].[Exercises] e on s.ExerciseId = e.Id
WHERE r.Id = @id
AND r.UserId = @userId
ORDER BY s.[Id]";

            var sets = await database.ExecuteQueryAsync<Exercise>(sql, new { userId, id });

            return sets.ToList();
        }

        public async Task<int> GetWeeksWorkoutsCountAsync(Guid userId, DateTime from, DateTime to)
        {
            var sql = @"
SELECT COUNT(*)
FROM [Workouts].[Routine]
WHERE Date BETWEEN @from AND @to";

            return await database.ExecuteQuerySingleAsync<int>(sql, new { userId, from, to });
        }

        public async Task<int> GetMonthsWorkoutsCountAsync(Guid userId, string month)
        {
            month = "%" + month + "%";
            var sql = @"
SELECT COUNT(*)
FROM [Workouts].[Routine]
WHERE Date LIKE @month";

            return await database.ExecuteQuerySingleAsync<int>(sql, new { userId, month });
        }
    }
}
