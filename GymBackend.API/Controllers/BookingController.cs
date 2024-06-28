using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Booking;
using GymBackend.Core.Domains.Booking;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IBookingService service;

        public BookingController(IAuthService authService, IBookingService service)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("timetable")]
        public async Task<List<BookingItem>> GetTimetable()
        {
            return await service.GetTimetable();
        }

        [HttpPost]
        public async Task<string> CreateBooking(int bookingId)
        {
            return await service.CreateBookingAsync(authService.CurrentUserId(), bookingId).ConfigureAwait(false);
        }
    }
}
