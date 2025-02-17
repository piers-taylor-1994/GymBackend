﻿using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Workouts
{
    public interface IWorkoutsStorage
    {
        Task<List<Exercise>> GetAllExercisesAsync();
        Task<List<Guid>> GetAllSearchExercisesAsync(MuscleGroup muscle);
        Task<Routine?> GetRoutineAsync(Guid userId, DateTime date, string table);
        Task<List<Set>> GetSetExerciseIdOrderByRoutineIdAsync(Guid routineId, string table);
        Task<List<SetArray>> GetSetsArrayBySetId(Guid setId, string table);
        Task<Routine> AddRoutineAsync(Guid id, Guid userId, DateTime date, string table);
        Task UpdateRoutineTimeAsync(Guid id, Guid userId, DateTime date, string table);
        Task AddExercisesToSetAsync(Guid id, Guid routineId, Guid exerciseId, int order, string table);
        Task AddExerciseSetFromArrayAsync(Guid setId, float weight, int sets, int reps, int order, string table);
        Task<List<Guid>> GetSetIdsFromRoutineId(Guid routineId, string table);
        Task DeleteSetsFromRoutineIdAsync(Guid routineId, List<Guid> setIds, string table);
        Task<List<Routine>> GetRoutinesAsync(Guid userId, string table);
        Task<MuscleArea> GetRoutineMuscleAreas(Guid routineId);
        Task<Set?> GetSetByExerciseIdAsync(Guid userId, Guid exerciseId);
        Task<List<MaxSet>> GetExerciseLeaderboardAsync(Guid exerciseId);

        Task<RoutineTemplate?> GetRoutineTemplateAsync(Guid userId, string name);
        Task<RoutineTemplate> GetRoutineTemplateAsync(Guid userId, Guid id);
        Task<RoutineTemplate> AddRoutineTemplateAsync(Guid id, Guid userId, string name);
        Task AddRoutineTemplateSetAsync(Guid routineTemplateId, Guid exerciseId);
        Task<List<RoutineTemplate>> GetRoutineTemplatesAsync(Guid userId);
        Task<List<Exercise>> GetRoutineTemplateSetsAsync(Guid userId, Guid id);
        Task UpdateRoutineTemplateNameAsync(Guid userId, Guid id, string name);
        Task DeleteRoutineTemplateSetsAsync(Guid id);
        Task DeleteRoutineTemplateAsync(Guid userId, Guid id);

        Task<int> GetWeeksWorkoutsCountAsync(Guid userId, DateTime startDate, DateTime endDate);
        Task<int> GetMonthsWorkoutsCountAsync(Guid userId, DateTime yearMonth);

        Task<Exercise> AddExerciseAsync(Exercise exercise);
        Task<ExerciseMuscle> AddExerciseMuscleAsync(Guid exerciseId, MuscleGroup muscle);
        Task<List<Routine>> GetRecentWorkoutsAsync();

        Task DeleteRoutineDataAsync(Guid userId, DateTime date, string table);
    }
}
