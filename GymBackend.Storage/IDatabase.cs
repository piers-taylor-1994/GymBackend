namespace YOTApp.Storage
{
    public interface IDatabase
    {
        Task<T?> ExecuteQuerySingleAsync<T>(string sql, object? param = null);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object? param = null);
        Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, string splitOn, object? param = null);

        Task ExecuteAsync(string sql, object? param = null);
    }
}