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
        public async Task<List<Exercise>> GetExercises()
        {
            return await service.GetExercisesAsync().ConfigureAwait(false);
        }

        [HttpGet("routine")]
        public async Task<RoutineSet?> GetRoutine(string userId)
        {
           return await service.GetRoutineAsync(userId).ConfigureAwait(false);
        }

        [HttpPost("routine")]
        public async Task<RoutineSet> PostRoutine(string userId, List<string> exerciseIds)
        {
            return await service.AddRoutineAsync(userId, exerciseIds).ConfigureAwait(false);
        }

        [HttpPut("routine/{id}")]
        public async Task<RoutineSet> UpdateRoutine(string id, List<SetUpdate> setList)
        {
            return await service.UpdateRoutineAsync(id, setList).ConfigureAwait(false);
        }
    }
}
