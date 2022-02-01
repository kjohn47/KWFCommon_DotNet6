namespace KWFCaching.Abstractions.Interfaces
{
    using KWFCaching.Abstractions.Models;

    public interface IKwfCacheBase
    {
        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class;
    }
}
