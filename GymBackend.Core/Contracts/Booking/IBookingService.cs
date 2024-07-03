using GymBackend.Core.Domains.Booking;

namespace GymBackend.Core.Contracts.Booking
{
    public interface IBookingService
    {
        public Task<List<BookingItem>> GetTimetable();
        public Task<string> CreateBookingAsync(Guid userId, int bookingId);
        public Task<List<int>> GetBookedClassesAsync(Guid userId);
    }
}
