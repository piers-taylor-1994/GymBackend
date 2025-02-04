using GymBackend.Core.Contracts;

namespace GymBackend.API.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor contextAccessor;

        public AuthService(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }
        public Guid CurrentUserId()
        {
            var context = contextAccessor.HttpContext ?? throw new Exception("Unable to access HttpContext");

            var claims = context.User.Claims;

            var userId = claims.Where((c, s) => c.Type == "sub").FirstOrDefault()?.Value ?? "C1FEF7F5-383B-4200-B498-C201A6AC1FEC";

            return Guid.Parse(userId);
        }
    }
}
