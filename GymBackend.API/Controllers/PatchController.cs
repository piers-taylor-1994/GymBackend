using GymBackend.Core.Contracts;
using GymBackend.Core.Contracts.Patch;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatchController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IPatchService service;

        public PatchController(IAuthService authService, IPatchService service)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("")]
        public async Task<bool> GetUserPatchRead()
        {
            return await service.GetUserPatchReadAsync(authService.CurrentUserId());
        }

        [HttpPost("")]
        public async Task<bool> SetUserPatchRead()
        {
            return await service.SetUserPatchReadAsync(authService.CurrentUserId());
        }
    }
}
