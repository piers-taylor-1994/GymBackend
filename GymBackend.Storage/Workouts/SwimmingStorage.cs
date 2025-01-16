using GymBackend.Core.Contracts.Swimming;
using GymBackend.Core.Domains.Swimming;
using GymBackend.Core.Domains.Workouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymBackend.Core.Domains.Swimming;
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

        public async Task AddASwimAsync(Guid id, Guid userId, DateTime date, int lengths, int timeSwimming, bool review, string explanation)
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
            
        }
    }
}
