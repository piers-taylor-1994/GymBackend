using GymBackend.Core.Contracts.Patch;
using YOTApp.Storage;

namespace GymBackend.Storage.Patch
{
    public class PatchStorage : IPatchStorage
    {
        private readonly IDatabase database;

        public PatchStorage(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<bool> GetUserPatchReadAsync(Guid userId)
        {
            var sql = "SELECT [PatchRead] FROM [Users].[Users] WHERE [Id] = @userId";

            return await database.ExecuteQuerySingleAsync<bool>(sql, new { userId });
        }

        public async Task<bool> SetUserPatchReadAsync(Guid userId)
        {
            var sql = @"
UPDATE [Users].[Users]
SET [PatchRead] = 1
WHERE [Id] = @userId";

            await database.ExecuteAsync(sql, new { userId });

            return await GetUserPatchReadAsync(userId);
        }
    }
}
