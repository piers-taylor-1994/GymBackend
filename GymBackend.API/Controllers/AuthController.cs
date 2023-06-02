using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly string[] Users = new[]
        {
        "Piers", "Jill", "Jacob", "Logan", "Trey"
        };

        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("")]
        public async Task<List<User>> GetUsers()
        {
            return await authService.GetUsersAsync().ConfigureAwait(false);
        }

        [HttpGet("user")]
        public async Task<User> GetUser()
        {
            return await authService.GetUserAsync().ConfigureAwait(false);
        }
    }
}