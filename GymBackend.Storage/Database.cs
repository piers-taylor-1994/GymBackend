using Dapper;
using Microsoft.Data.SqlClient;

namespace GymBackend.Storage
{
    public class Database : IDatabase
    {
        private readonly string connectionString;
        public Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<T?> ExecuteQuerySingleAsync<T>(string sql, object? param = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);

                return await connection.QuerySingleAsync<T>(sql, param);
            }
            catch (InvalidOperationException)
            {
                return default;
            }
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object? param = null)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<T>(sql, param);
        }

        public async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, string splitOn, object? param = null)
        {
            using var connection = new SqlConnection(connectionString);

            var result = await connection.QueryAsync(sql, map: map, param: param, splitOn: splitOn);

            return result;
        }

        public async Task ExecuteAsync(string sql, object? param = null)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(sql, param);
        }
    }
}
