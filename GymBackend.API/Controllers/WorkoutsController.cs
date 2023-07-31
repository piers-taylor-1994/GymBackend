using GymBackend.API.Models;
using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
        public async Task<RoutineSet> AddRoutine(List<ExerciseSet> sets)
        {
            return await service.AddRoutineAsync(authService.CurrentUserId(), sets).ConfigureAwait(false);
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

        [HttpPost("routine/template")]
        public async Task<RoutineTemplate> AddRoutineTemplate([FromBody]AddRoutineTemplate routineTemplate)
        {
            return await service.AddRoutineTemplateAsync(authService.CurrentUserId(), routineTemplate.Name, routineTemplate.ExerciseIds).ConfigureAwait(false);
        }

        [HttpGet("routine/templates")]
        public async Task<List<RoutineTemplate>> GetRoutineTemplates()
        {
            return await service.GetRoutineTemplatesAsync(authService.CurrentUserId()).ConfigureAwait(false);
        }

        [HttpGet("routine/template/{id}")]
        public async Task<List<Exercise>> GetRoutineTemplateSets(string id)
        {
            return await service.GetRoutineTemplateSetsAsync(authService.CurrentUserId(), id).ConfigureAwait(false);
        }

        [HttpGet("count")]
        public async Task<WorkoutsCount> GetWorkoutsCount()
        {
            return await service.GetWorkoutsCountAsync(authService.CurrentUserId()).ConfigureAwait(false);
        }
    }
}
