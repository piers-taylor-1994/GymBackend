namespace GymBackend.Core.Contracts.Swimming
{
    public interface ISwimmingService
    {
        public Task AddASwimAsync(Guid userId, DateTime dateTime, int lengths, int timeSwimming, bool review, string explanation);
    }
}
