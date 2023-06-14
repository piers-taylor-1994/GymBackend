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

            var userId = claims.Where((c, s) => c.Type == "sub").FirstOrDefault() ?? throw new Exception("Sub claim was not present");

            return Guid.Parse(userId.Value);
        }
    }
}
