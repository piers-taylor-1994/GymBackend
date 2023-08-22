namespace YOTApp.Storage
{
    public interface IDatabase
    {
        Task<T?> ExecuteQuerySingleAsync<T>(string sql, object? param = null); //Return singular
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object? param = null); //Return list
        Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, string splitOn, object? param = null); //Return list with dictionary

        Task ExecuteAsync(string sql, object? param = null); //No return
    }
}