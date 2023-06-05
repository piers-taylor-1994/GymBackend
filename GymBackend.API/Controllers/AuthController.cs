using GymBackend.API.Models;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace GymBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("password/hash")]
        public string GetPasswordHash(string password)
        {
            return authService.GetPasswordHashAsync(password);
        }

        [HttpGet("")]
        public async Task<List<User>> GetUsers()
        {
            return await authService.GetUsersAsync().ConfigureAwait(false);
        }

        [HttpPost("logon")]
        public async Task<ActionResult<string>> Logon(Logon logon)
        {
            var authUser = await authService.LogonAsync(logon.Username, logon.Password).ConfigureAwait(false);

            if (authUser == null) return new UnauthorizedResult();

            var token = RandomNumberGenerator.GetInt32(10000, 100000).ToString();

            return token;
        }
    }
}