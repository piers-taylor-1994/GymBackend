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

        [HttpGet("search/{muscle}")]
        public async Task<List<Guid>> SearchExercises(MuscleGroup muscle)
        {
            return await service.SearchExercisesAsync(muscle).ConfigureAwait(false);
        }

        [HttpGet("routine")]
        public async Task<RoutineSet?> GetRoutine()
        {
            return await service.GetRoutineAsync(authService.CurrentUserId()).ConfigureAwait(false);
        }

        [HttpPost("routine")]
        public async Task<RoutineSet> PostRoutine(List<string> exerciseIds)
        {
            return await service.AddRoutineAsync(authService.CurrentUserId(), exerciseIds).ConfigureAwait(false);
        }

        [HttpPut("routine/{id}")]
        public async Task<RoutineSet> UpdateRoutine(string id, List<SetUpdate> setList)
        {
            return await service.UpdateRoutineAsync(id, setList).ConfigureAwait(false);
        }

        [HttpDelete("routine/set/{id}")]
        public async Task RemoveSetFromRoutine(string id)
        {
            await service.DeleteSetFromRoutineAsync(authService.CurrentUserId(), id).ConfigureAwait(false);
        }

        [HttpPut("routine/set/order")]
        public async Task<Dictionary<Guid, int>> UpdateSetOrder([FromBody]Dictionary<Guid, int> setDict)
        {
            return await service.UpdateSetOrderAsync(setDict).ConfigureAwait(false);
        }

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

        [HttpPost("routine/last")]
        public async Task<List<Set>> GetLastSetForExercises([FromBody]List<string> exerciseIds)
        {
            return await service.GetLastSetForExercisesAsync(authService.CurrentUserId(), exerciseIds).ConfigureAwait(false);
        }

        [HttpGet("routine/leaderboard/{exerciseId}")]
        public async Task<List<MaxSet>> GetExerciseLeaderboard(string exerciseId)
        {
            return await service.GetExerciseLeaderboardAsync(exerciseId).ConfigureAwait(false);
        }
    }
}
