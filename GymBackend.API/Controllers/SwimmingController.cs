using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Contracts.Swimming;
using GymBackend.Core.Domains.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SwimmingController : ControllerBase
    {
        private readonly ISwimmingService service;
        private readonly IAuthService authService;
        private readonly IAuthManager authManager;

        public SwimmingController(ISwimmingService service, IAuthService authService, IAuthManager authManager)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.authManager = authManager ?? throw new ArgumentNullException(nameof(authManager));
        }
        

        [HttpPost("AddSwim")]
        public async Task<Swimming> AddASwim(int lengths, int timeSwimming, bool review, string? explanation)
        {
            DateTime today = DateTime.Now;
            
            return await service.AddASwimAsync(authService.CurrentUserId(),today,lengths,timeSwimming,review,explanation).ConfigureAwait(false);
        }
        [HttpGet("TodaysSwim")]
        public async Task<Swimming?> GetTodaysSwim()
        {
            var currentUser = authService.CurrentUserId();
            return await service.GetTodaysSwimAsync(currentUser);
        }
        [HttpGet("RecentSwims")]
        public async Task<List<Swimming>> GetRecentSwims()
        {
            var currentUser = authService.CurrentUserId();
            return await service.GetRecentSwimsAsync(currentUser);
        }
        [HttpGet("SelectedSwim/{id}")]
        public async Task<Swimming> FindASwim(Guid id)
        {
            var currentUser = authService.CurrentUserId();
            //Guid id = Guid.Parse("0C2FB9D6-C991-473A-A326-1A0719C47F1F");
            return await service.FindASwimAsync(currentUser, id);
        }
        [HttpPut("UpdateSwim/{id}")]
        public async Task<Swimming> UpdateASwim(Guid id, int lengths, int timeSwimming, bool review, string? explanation)
        {
            var currentUser = authService.CurrentUserId();
            return await service.UpdateASwimAsync(currentUser, id, lengths, timeSwimming, review, explanation);
        }
        [HttpDelete("DeleteSwim/{id}")]
        public async Task DeleteASwim(Guid id)
        {
            var currentUser = authService.CurrentUserId();
            await service.DeleteASwimAsync(currentUser, id);
        }
        [HttpGet("Count")]
        public async Task<WorkoutsCount> GetSwimCount()
        {
            var currentUser = authService.CurrentUserId();
            return await service.GetSwimCountAsync(currentUser);
        }
        [HttpGet("AllSwims")]
        public async Task<List<Swimming>> GetAllSwims()
        {
            var currentUser = authService.CurrentUserId();
            return await service.GetAllSwimsAsync(currentUser);
        }


    }
}
