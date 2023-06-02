using GymBackend.Core.Domains;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthService
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync();
    }
}
