namespace KWFCaching.Memory.Interfaces
{
    using KWFCaching.Abstractions.Interfaces;
    using KWFCaching.Abstractions.Models;

    using Microsoft.Extensions.Caching.Memory;

    public interface IKwfCacheOnMemory : IKwfCacheBase, IMemoryCache
    {
        CachedResult<TResult> GetCachedItem<TResult>(string key) where TResult : class;

        void SetCachedItem<TItem>(string key, TItem value, string? settingsKey = null) where TItem : class;

        void RemoveCachedItem(string key);
    }
}
