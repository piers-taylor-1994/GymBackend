using GymBackend.Core.Contracts.Swimming;
using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Service.Workouts
{
    public class SwimmingService : ISwimmingService
    {
        private readonly ISwimmingStorage storage;

        public SwimmingService(ISwimmingStorage storage)
        {
            this.storage = storage;
        }

        public async Task<Swimming> AddASwimAsync(Guid userId, DateTime dateTime, int lengths, int timeSwimming, bool review, string explanation)
        {
            Guid id = Guid.NewGuid();
            return await storage.AddASwimAsync(id, userId, dateTime, lengths, timeSwimming, review, explanation);
        }
        public async Task<Swimming> GetRecentSwimAsync(Guid userId)
        {
            return await storage.GetRecentSwimAsync(userId);
        }
        public async Task<List<Swimming>> GetRecentSwimsAsync(Guid userId)
        {
            return await storage.GetRecentSwimsAsync(userId);
        }
        public async Task<Swimming> FindASwimAsync(Guid userId, Guid id)
        {
            return await storage.FindASwimAsync(userId, id);
        }
        public async Task<Swimming> UpdateASwimAsync(Guid userId, Guid id, int lengths, int timeSwimming, bool review, string? explanation)
        {
            return await storage.UpdateASwimAsync(userId, id, lengths, timeSwimming, review, explanation);
        }
        public async Task DeleteASwimAsync(Guid userId, Guid id)
        {
            await storage.DeleteASwimAsync(userId, id);
        }
        public async Task<WorkoutsCount> GetSwimCountAsync(Guid userId)
        {
            int i = 0;
            var today = DateTime.Now;
            while (today.DayOfWeek != DayOfWeek.Monday)
            {
                today = today.AddDays(1);
                i++;
            }
            var minusWeek = i + 7;
            var thisWeek = await storage.GetWeeksSwimsAsync(userId, DateTime.Now.AddDays(-i), today);
            var lastWeek = await storage.GetWeeksSwimsAsync(userId, DateTime.Now.AddDays(-minusWeek), DateTime.Now.AddDays(-i));
            var thisMonth = await storage.GetMonthsSwimsAsync(userId, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01));
            var lastMonth = await storage.GetMonthsSwimsAsync(userId, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01).AddMonths(-1));

            return new WorkoutsCount(thisWeek, lastWeek, thisMonth, lastMonth);

        }
        public async Task<List<Swimming>> GetAllSwimsAsync(Guid userId)
        {
            return await storage.GetAllSwimsAsync(userId);
        }
    }
}
