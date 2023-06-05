using GymBackend.Core.Domains.User;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthService
    {
        string GetPasswordHashAsync(string password);
        Task<AuthUser?> LogonAsync(string username, string password);
        Task<List<User>> GetUsersAsync();
        Task<string> IssueToken(AuthUser user);
    }
}
