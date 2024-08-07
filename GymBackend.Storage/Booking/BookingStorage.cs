using GymBackend.Core.Contracts.Booking;

namespace GymBackend.Storage.Booking
{
    public class BookingStorage : IBookingStorage
    {
        private readonly IDatabase database;

        public BookingStorage(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<string> GetTokenAsync()
        {
            var sql = "SELECT [Token] FROM [Workouts].[Token]";
            return await database.ExecuteQuerySingleAsync<string>(sql) ?? "";
        }

        public async Task<string> SetTokenAsync(string token)
        {
            var sql = @"
UPDATE [Workouts].[Token]
SET [Token] = @token
WHERE [Id] = 0";
            await database.ExecuteAsync(sql, new { token });
            return await GetTokenAsync();
        }
    }
}
