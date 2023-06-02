using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains;

namespace GymBackend.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthStorage storage;

        public AuthService(IAuthStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<User> GetUserAsync()
        {
            return await storage.GetUserAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await storage.GetUsersAsync();
        }
    }
}
