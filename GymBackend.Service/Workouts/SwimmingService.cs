using GymBackend.Core.Contracts.Swimming;

namespace GymBackend.Service.Workouts
{
    public class SwimmingService : ISwimmingService
    {
        private readonly ISwimmingStorage storage;

        public SwimmingService(ISwimmingStorage storage)
        {
            this.storage = storage;
        }

        public async Task AddASwimAsync(Guid userId, DateTime dateTime, int lengths, int timeSwimming, bool review, string explanation)
        {
            Guid id = Guid.NewGuid();
            await storage.AddASwimAsync(id, userId, dateTime, lengths, timeSwimming, review, explanation);
        }
    }
}
