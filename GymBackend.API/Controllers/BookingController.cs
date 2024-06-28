using GymBackend.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IAuthService authService;

        public BookingController(IAuthService authService)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("timetable")]
        public async Task GetTimetable()
        {

        }
    }
}
