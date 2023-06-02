using GymBackend.Core.Domains;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthStorage
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync();
    }
}
