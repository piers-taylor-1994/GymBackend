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
        public async Task<List<Swimming>> GetAllSwimsAsync(Guid userId)
        {
            return await storage.GetAllSwimsAsync(userId);
        }
    }
}
