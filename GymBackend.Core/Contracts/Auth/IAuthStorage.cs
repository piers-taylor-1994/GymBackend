using GymBackend.Core.Domains;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthStorage
    {
        Task<AuthUser?> FindUserAsync(string username);
        Task<List<User>> GetUsersAsync();
    }
}
