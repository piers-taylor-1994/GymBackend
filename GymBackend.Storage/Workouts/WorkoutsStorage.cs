﻿using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;
using YOTApp.Storage;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<Routine?> GetRoutineAsync(Guid userId, DateTime date)
        {
            var sql = "SELECT * FROM [Workouts].[Routine] WHERE [UserId] = @userId AND [Date] = @date";

            return await database.ExecuteQuerySingleAsync<Routine>(sql, new { userId, date }) ?? null;
        }

        public async Task<Routine?> GetRoutineAsync(Guid id)
        {
            var sql = "SELECT * FROM [Workouts].[Routine] WHERE [Id] = @id";

            return await database.ExecuteQuerySingleAsync<Routine>(sql, new { id }) ?? null;
        }

        public async Task<List<Set>> GetSetsByRoutineIdAsync(Guid routineId)
        {
            var sql = @"
SELECT s.[Id], e.[Id] as ExerciseId, e.[MuscleGroupId] as MuscleGroup, e.[Name], e.[Description], s.[Weight], s.[Sets], s.[Reps]
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

        public async Task DeleteSetFromRoutineAsync(Guid routineId, Guid setId)
        {
            var sql = "DELETE FROM [Workouts].[Sets] WHERE [RoutineId] = @routineId AND [Id] = @setId";

            await database.ExecuteAsync(sql, new { routineId, setId });
        }

        public async Task<List<Set>> UpdateSetsForRoutineAsync(Guid routineId, SetUpdate set)
        {
            var updateSql = @"
UPDATE 
    [Workouts].[Sets]
SET
    [Weight] = @Weight,
    [Sets] = @Sets,
    [Reps] = @Reps
WHERE [Id] = @Id";

            await database.ExecuteAsync(updateSql, set);

            return await GetSetsByRoutineIdAsync(routineId);
        }

        public async Task<List<Routine>> GetRoutinesAsync(Guid userId)
        {
            var sql = "SELECT * FROM [Workouts].[Routine] WHERE [UserId] = @userId ORDER BY [Date] DESC";

            var routines = await database.ExecuteQueryAsync<Routine>(sql, new { userId });

            return routines.ToList();
        }
    }
}
