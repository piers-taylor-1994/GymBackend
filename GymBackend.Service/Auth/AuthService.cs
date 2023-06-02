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
        public async Task<List<Users>> GetUsersAsync()
        {
            return await storage.GetUsersAsync();
        }
    }
}
