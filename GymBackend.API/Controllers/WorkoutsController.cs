using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.User;
using GymBackend.Core.Domains.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutsService service;

        public WorkoutsController(IWorkoutsService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }
        [HttpGet("")]
        public async Task<List<Exercises>> GetUsers()
        {
            return await service.GetExercisesAsync().ConfigureAwait(false);
        }
    }
}
