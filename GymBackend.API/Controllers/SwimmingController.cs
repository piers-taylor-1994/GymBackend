using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Contracts.Swimming;
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

        [HttpPost("")]
        public async Task AddASwim()
        {
            await service.AddASwimAsync(Guid.NewGuid(),DateTime.Now,0,0,true,null).ConfigureAwait(false);
        }
    }
}
