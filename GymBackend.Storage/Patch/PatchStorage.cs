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

        public async Task<float> GetUserPatchReadAsync(Guid userId)
        {
            var sql = "SELECT [Patch] FROM [Users].[Users] WHERE [Id] = @userId";

            return await database.ExecuteQuerySingleAsync<float>(sql, new { userId });
        }

        public async Task<float> SetUserPatchReadAsync(Guid userId, float patch)
        {
            var sql = @"
UPDATE [Users].[Users]
SET [Patch] = @patch
WHERE [Id] = @userId";

            await database.ExecuteAsync(sql, new { userId, patch });

            return await GetUserPatchReadAsync(userId);
        }
    }
}
