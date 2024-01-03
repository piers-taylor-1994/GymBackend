using GymBackend.Core.Domains.User;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthManager
    {
        string GetPasswordHashAsync(string password);
        string CreateRandomPasswordAsync();
        Task<AuthUser?> LogonAsync(string username, string password);
        Task<List<User>> GetUsersAsync();
        Task<string> IssueTokenAsync(AuthUser user);
        Task<AuthUser?> GetAuthUserAsync(string username);
        Task<Dictionary<Guid, string>> GetUsernameAsync(IEnumerable<Guid> ids);
    }
}
