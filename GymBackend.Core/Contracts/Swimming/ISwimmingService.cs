using GymBackend.Core.Domains.Workouts;

namespace GymBackend.Core.Contracts.Swimming
{
    public interface ISwimmingService
    {
        public Task<Domains.Workouts.Swimming> AddASwimAsync(Guid userId, DateTime dateTime, int lengths, int timeSwimming, bool review, string explanation);
        public Task<Domains.Workouts.Swimming> GetRecentSwimAsync(Guid userId);
        public Task<List<Domains.Workouts.Swimming>> GetRecentSwimsAsync(Guid userId);
        public Task<Domains.Workouts.Swimming> FindASwimAsync(Guid userId, Guid id);
        public Task<Domains.Workouts.Swimming> UpdateASwimAsync(Guid userId, Guid id, int lengths, int timeSwimming, bool review, string? explanation);
    }
}
