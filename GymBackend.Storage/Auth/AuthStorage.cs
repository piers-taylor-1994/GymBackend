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
        public async Task<List<Users>> GetUsersAsync()
        {
            var sql = "SELECT * FROM [Users].[Users]";
            var users = await database.ExecuteQueryAsync<Users>(sql);
            return users.ToList();
        }
    }
}
