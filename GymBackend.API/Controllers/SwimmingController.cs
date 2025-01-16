using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Contracts.Swimming;
using GymBackend.Core.Domains.Swimming;
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
        

        [HttpPost("add")]
        public async Task AddASwim(int lengths, int timeSwimming, bool review, string? explanation)
        {
            DateTime today = DateTime.Now;
            var currentUser = authService.CurrentUserId;
            await service.AddASwimAsync(authService.CurrentUserId(),today,lengths,timeSwimming,review,explanation).ConfigureAwait(false);
        }
    }
}
