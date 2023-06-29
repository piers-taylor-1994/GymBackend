using GymBackend.API.Models;
using GymBackend.Core.Contracts;
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
        private readonly IAuthService authService;

        public WorkoutsController(IWorkoutsService service, IAuthService authService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }
        [HttpGet("")]
        public async Task<List<Exercise>> GetExercises()
        {
            return await service.GetExercisesAsync().ConfigureAwait(false);
        }

        [HttpGet("routine")]
        public async Task<RoutineSet?> GetRoutine()
        {
            return await service.GetRoutineAsync(authService.CurrentUserId()).ConfigureAwait(false);
        }

        [HttpPost("routine")]
        public async Task<RoutineSet> AddRoutine(List<ExerciseSet> exerciseList)
        {
            return await service.AddRoutineAsync(authService.CurrentUserId(), exerciseList).ConfigureAwait(false);
        }

        //[HttpPut("routine/{id}")]
        //public async Task<RoutineSet> UpdateRoutine(string id, List<SetUpdate> setList)
        //{
        //    return await service.UpdateRoutineAsync(id, setList).ConfigureAwait(false);
        //}

        [HttpDelete("routine/set/{exerciseId}")]
        public async Task RemoveSetFromRoutine(string exerciseId)
        {
            await service.DeleteSetFromRoutineAsync(authService.CurrentUserId(), exerciseId).ConfigureAwait(false);
        }

        //[HttpPut("routine/set/order")]
        //public async Task<Dictionary<Guid, int>> UpdateSetOrder([FromBody]Dictionary<Guid, int> setDict)
        //{
        //    return await service.UpdateSetOrderAsync(setDict).ConfigureAwait(false);
        //}

        [HttpGet("routine/history")]
        public async Task<List<Routine>> GetRoutinesHistory()
        {
            return await service.GetRoutinesHistoryAsync(authService.CurrentUserId()).ConfigureAwait(false);
        }

        [HttpGet("routine/history/{id}")]
        public async Task<RoutineSet> GetRoutineHistory(string id)
        {
            return await service.GetRoutineHistoryAsync(id).ConfigureAwait(false);
        }
    }
}
