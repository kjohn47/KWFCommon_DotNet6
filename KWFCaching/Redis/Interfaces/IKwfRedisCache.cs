namespace KWFCaching.Redis.Interfaces
{
    using KWFCaching.Abstractions.Interfaces;
    using KWFCaching.Abstractions.Models;

    using Microsoft.Extensions.Caching.Distributed;

    public interface IKwfRedisCache : IKwfCacheBase, IDistributedCache
    {
        Task<CachedResult<TResult>> GetCachedItemAsync<TResult>(
            string key,
            CancellationToken? cancellationToken = null) where TResult : class;

        Task SetCachedItemAsync<TItem>(
            string key,
            TItem value,
            string? settingsKey = null,
            CancellationToken? cancellationToken = null) where TItem : class;

        Task RemoveCachedItemAsync(
            string key,
            CancellationToken? cancellationToken = null);
    }
}
