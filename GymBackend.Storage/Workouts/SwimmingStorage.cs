﻿using GymBackend.Core.Contracts.Swimming;
using GymBackend.Core.Domains.Workouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymBackend.Core.Contracts.Swimming;

namespace GymBackend.Storage.Workouts
{
    public class SwimmingStorage : ISwimmingStorage
    {
        private readonly IDatabase database;

        public SwimmingStorage(IDatabase database)
        {
            this.database = database;
        }

        public async Task<Swimming> AddASwimAsync(Guid id, Guid userId, DateTime date, int lengths, int timeSwimming, bool review, string explanation)
        {
            var sqlCreate = $@"
INSERT INTO [Workouts].[Swimming] ([Id],[UserId], [Date], [Lengths], [TimeSwimming], [Review], [Explanation])
VALUES (
@id,
@userId,
@date,
@lengths,
@timeSwimming,
@review,
@explanation
)
";
            await database.ExecuteAsync(sqlCreate, new { id, userId, date, lengths, timeSwimming, review, explanation });
            return await FindASwimAsync(userId, id);
            
        }
        public async Task<Swimming> GetRecentSwimAsync(Guid userId)
        {
            var sqlGet = @"
SELECT TOP 1 * FROM [Workouts].[Swimming] WHERE [UserId] = @userId 
";
            var swim = await database.ExecuteQuerySingleAsync<Swimming>(sqlGet, new { userId });
            return swim;
        }
        public async Task<List<Swimming>> GetRecentSwimsAsync(Guid userId)
        {
            var sqlGet = @"
SELECT TOP(3) * FROM [Workouts].[Swimming]
WHERE [UserId] = @userId
ORDER BY [Date] DESC";
            var swims = await database.ExecuteQueryAsync<Swimming>(sqlGet, new { userId });
            return swims.ToList();

        }
        public async Task<Swimming> FindASwimAsync(Guid userId, Guid id)
        {
            var sqlGet = @"
SELECT TOP(1) * FROM [Workouts].[Swimming]
WHERE [UserId] = @userId AND [Id] = @id";
            var swim = await database.ExecuteQuerySingleAsync<Swimming>(sqlGet, new { userId, id });
            return swim;
        }
        public async Task<Swimming> UpdateASwimAsync(Guid userId, Guid id, int lengths, int timeSwimming, bool review, string? explanation)
        {
            var sqlPut = @"
UPDATE [Workouts].[Swimming]
SET [Lengths] = @lengths , [TimeSwimming] = @timeSwimming , [Review] = @review , [Explanation] = @explanation
WHERE [UserId] = @userId AND [Id] = @id
";
            await database.ExecuteQueryAsync<Swimming>(sqlPut, new { userId, id, lengths, timeSwimming, review, explanation });
            return await FindASwimAsync(userId, id);
        }
        public async Task DeleteASwimAsync(Guid userId, Guid id)
        {
            var sqlDelete = @"
DELETE [Workouts].[Swimming] WHERE [UserId] = @userId AND [Id] = @id
";
            await database.ExecuteAsync(sqlDelete, new { userId, id });
        }
        public async Task<List<Swimming>> GetAllSwimsAsync(Guid userId)
        {
            var sqlGet = @"
SELECT * FROM [Workouts].[Swimming] WHERE [UserId] = @userId";
            var allSwims = await database.ExecuteQueryAsync<Swimming>(sqlGet, new { userId });
            return allSwims.ToList();
        }
    }
}
