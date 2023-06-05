using GymBackend.Core.Domains;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthService
    {
        string GetPasswordHashAsync(string password);
        Task<List<User>> GetUsersAsync();
        Task<AuthUser?> LogonAsync(string username, string password);
    }
}
