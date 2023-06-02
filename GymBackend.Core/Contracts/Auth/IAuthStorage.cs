using GymBackend.Core.Domains;

namespace GymBackend.Core.Contracts.Auth
{
    public interface IAuthStorage
    {
        Task<List<Users>> GetUsersAsync();
    }
}
