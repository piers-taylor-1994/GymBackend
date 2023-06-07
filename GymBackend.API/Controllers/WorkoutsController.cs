using GymBackend.Core.Contracts.Workouts;
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
        public async Task<List<Exercises>> GetExercises()
        {
            return await service.GetExercisesAsync().ConfigureAwait(false);
        }

        [HttpGet("routine")]
        public async Task<Routine> GetRoutine(string userId)
        {
            return await service.GetRoutineAsync(userId).ConfigureAwait(false);
        }
    }
}
