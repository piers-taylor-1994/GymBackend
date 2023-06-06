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
            var sql = "SELECT * FROM [Workouts].[Exercises]";
            var exercises = await database.ExecuteQueryAsync<Exercises>(sql);
            return exercises.ToList();
        }
    }
}
