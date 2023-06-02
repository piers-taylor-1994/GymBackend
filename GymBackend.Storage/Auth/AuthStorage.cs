using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains;
using YOTApp.Storage;

namespace GymBackend.Storage.Auth
{
    public class AuthStorage : IAuthStorage
    {
        private readonly IDatabase database;

        public AuthStorage(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<User> GetUserAsync()
        {
            var sql = "SELECT * FROM [Users].[Users] WHERE Username = 'piers'";
            return await database.ExecuteQuerySingleAsync<User>(sql);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var sql = "SELECT * FROM [Users].[Users]";
            var users = await database.ExecuteQueryAsync<User>(sql);
            return users.ToList();
        }
    }
}
