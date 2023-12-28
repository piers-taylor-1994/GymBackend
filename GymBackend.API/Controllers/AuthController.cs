using GymBackend.API.Models;
using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymBackend.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthManager service;

        public AuthController(ILogger<AuthController> logger, IAuthManager authService)
        {
            _logger = logger;
            this.service = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("password/hash")]
        public string GetPasswordHash(string password)
        {
            return service.GetPasswordHashAsync(password);
        }

        [HttpPost("logon")]
        public async Task<ActionResult<string>> Logon(Logon logon)
        {
            var authUser = await service.LogonAsync(logon.Username, logon.Password).ConfigureAwait(false);

            if (authUser == null) return new UnauthorizedResult();

            var token = await service.IssueTokenAsync(authUser);

            return token;
        }

        [HttpGet("")]
        public async Task<List<User>> GetUsers()
        {
            return await service.GetUsersAsync().ConfigureAwait(false);
        }

        [HttpPost("token/resend/{username}")]
        public async Task<ActionResult<string>> ResendToken(string username)
        {
            var authUser = await service.GetAuthUserAsync(username).ConfigureAwait(false);

            if (authUser == null) return new UnauthorizedResult();

            var token = await service.IssueTokenAsync(authUser);

            return token;
        }
    }
}