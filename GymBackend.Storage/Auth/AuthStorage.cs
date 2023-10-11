using GymBackend.Core.Contracts.Auth;
using GymBackend.Core.Domains.User;

namespace GymBackend.Storage.Auth
{
    public class AuthStorage : IAuthStorage
    {
        private readonly IDatabase database;

        public AuthStorage(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<AuthUser?> FindUserAsync(string username)
        {
            var sql = "SELECT [Id], [Password] as PasswordHash FROM [Users].[Users] WHERE Username = @username";
            return await database.ExecuteQuerySingleAsync<AuthUser>(sql, new { username });
        }

        public async Task<AuthUser?> FindUserAsync(Guid userId)
        {
            var sql = "SELECT [Id], [Password] as PasswordHash FROM [Users].[Users] WHERE Username = @username";
            return await database.ExecuteQuerySingleAsync<AuthUser>(sql, new { userId });
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var sql = "SELECT * FROM [Users].[Users]";
            var users = await database.ExecuteQueryAsync<User>(sql);
            return users.ToList();
        }

        public async Task<Name?> GetNameByIdAsync(Guid id)
        {
            var sql = "SELECT [FirstName], [LastName] FROM [Users].[Users] WHERE Id = @id";
            return await database.ExecuteQuerySingleAsync<Name>(sql, new { id });
        }

        public async Task<string?> GetUsernameAsync(Guid id)
        {
            var sql = "SELECT [Username] FROM [Users].[Users] WHERE Id = @id";
            return await database.ExecuteQuerySingleAsync<string>(sql, new { id });
        }
    }
}
