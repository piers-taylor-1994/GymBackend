using GymBackend.Core.Domains.User;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthManager
    {
        string GetPasswordHashAsync(string password);
        Task<AuthUser?> LogonAsync(string username, string password);
        Task<List<User>> GetUsersAsync();
        Task<string> IssueToken(AuthUser user);
        Task<AuthUser?> GetAuthUser(string username);
    }
}
