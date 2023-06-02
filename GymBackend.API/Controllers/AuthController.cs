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

        //[HttpGet("Get")]
        //public IEnumerable<Users> Get()
        //{
        //    return Users.Select(name => new Users
        //    {
        //        Name = name
        //    })
        //    .ToArray();
        //}

        [HttpGet("Get")]
        public async Task<List<Users>> Get()
        {
            return await authService.GetUsersAsync();
        }
    }
}