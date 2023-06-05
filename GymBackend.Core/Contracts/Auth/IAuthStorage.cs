using GymBackend.Core.Domains.User;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthStorage
    {
        Task<AuthUser?> FindUserAsync(string username);
        Task<List<User>> GetUsersAsync();
        Task<Name?> GetNameByIdAsync(Guid id);
        Task<string?> GetUsernameAsync(Guid id);
    }
}
