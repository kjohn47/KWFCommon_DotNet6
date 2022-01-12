namespace KWFCaching.Memory.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using KWFCaching.Memory.Implementation;

    using Microsoft.Extensions.Caching.Memory;

    public interface IKwfCacheOnMemory : IMemoryCache
    {
        CachedResult<TResult> GetCachedItem<TResult>(string key) where TResult : class;

        void SetCachedItem<TItem>(string key, TItem value, string? settingsKey = null) where TItem : class;

        void RemoveCachedItem(string key);

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler) where TResult : class;

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler) where TResult : class;

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class;

        Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class;

    }
}
