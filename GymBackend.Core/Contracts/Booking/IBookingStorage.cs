namespace GymBackend.Core.Contracts.Booking
{
    public interface IBookingStorage
    {
        Task<string> GetTokenAsync();
        Task<string> SetTokenAsync (string token);
    }
}
