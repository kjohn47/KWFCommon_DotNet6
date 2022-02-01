namespace KWFCaching.Memory.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using KWFCaching.Abstractions.Interfaces;
    using KWFCaching.Abstractions.Models;
    using KWFCaching.Memory.Interfaces;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class KwfCacheOnMemory : MemoryCache, IKwfCacheOnMemory
    {
        private readonly IDictionary<string, CacheKeyEntry> _cachedKeySettings;

        private readonly TimeSpan _defaultCacheExpiration;

        public KwfCacheOnMemory(Microsoft.Extensions.Options.IOptions<KwfCacheOptions> options) : base(options)
        {
            _cachedKeySettings = options?.Value?.CachedKeySettings ?? new Dictionary<string, CacheKeyEntry>();
            _defaultCacheExpiration = options?.Value?.DefaultCacheExpiration ?? new TimeSpan(0, 60, 0);
        }

        public KwfCacheOnMemory(Microsoft.Extensions.Options.IOptions<KwfCacheOptions> options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            _cachedKeySettings = options?.Value?.CachedKeySettings ?? new Dictionary<string, CacheKeyEntry>();
            _defaultCacheExpiration = options?.Value?.DefaultCacheExpiration ?? new TimeSpan(0, 60, 0);
        }

        public CachedResult<TResult> GetCachedItem<TResult>(string key)
            where TResult : class
        {
            return GetAndValidateCacheValue<TResult>(key);
        }

        public void SetCachedItem<TItem>(string key, TItem value, string? settingsKey = null)
            where TItem : class
        {
            SetCacheValue(value, key, settingsKey);
        }

        public void RemoveCachedItem(string key)
        {
            Remove(key);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, null, null, cancellationToken);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, null, settingsKey, cancellationToken);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, null, null, cancellationToken);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, null, settingsKey, cancellationToken);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, fetchDataResultHandler, null, cancellationToken);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, fetchDataResultHandler, settingsKey, cancellationToken);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, fetchDataResultHandler, null, cancellationToken);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(
            string key,
            string settingsKey,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>> fetchDataResultHandler,
            CancellationToken? cancellationToken = null) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, fetchDataResultHandler, settingsKey, cancellationToken);
        }

        private async Task<TResult> GetHandledItemFromCacheAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<CachedResult<TResult>, TResult> resultHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>>? fetchDataResultHandler = null,
            string? settingsKey = null,
            CancellationToken? cancellationToken = null)
            where TResult : class
        {
            return resultHandler(await GetItemFromCacheAsync(key, fetchDataHandler, fetchDataResultHandler, settingsKey, cancellationToken));
        }

        private async Task<CachedResult<TResult>> GetItemFromCacheAsync<TResult>(
            string key,
            Func<CancellationToken?, Task<TResult>> fetchDataHandler,
            Func<TResult, IKwfCacheHandlerResult<TResult>>? fetchDataResultHandler = null,
            string? settingsKey = null,
            CancellationToken? cancellationToken = null)
            where TResult : class
        {
            CachedResult<TResult> cachedValue = GetAndValidateCacheValue<TResult>(key);
            if (cachedValue.CacheMiss)
            {
                var response = await fetchDataHandler(cancellationToken);
                if (fetchDataResultHandler != null)
                {
                    var handledResponse = fetchDataResultHandler(response);
                    if (handledResponse.PreventCacheWrite)
                    {
                        cachedValue.Result = handledResponse.Result;
                        return cachedValue;
                    }
                    response = handledResponse.Result;
                }

                cachedValue.Result = SetCacheValue(response, key, settingsKey);
            }

            return cachedValue;
        }

        private CachedResult<TResult> GetAndValidateCacheValue<TResult>(string key)
            where TResult : class
        {
            if (TryGetValue(key, out object value))
            {
                if (value is not TResult parsedValue)
                {
                    throw new KwfOnMemoryCacheException(
                        Constants.MemoryCacheObjectTypeExCode,
                        string.Format(Constants.MemoryCacheObjectTypeExMessage, key, typeof(TResult).Name));
                }

                return new CachedResult<TResult>
                {
                    Result = parsedValue
                };
            }

            return new CachedResult<TResult>()
            {
                CacheMiss = true
            };
        }

        private TResult SetCacheValue<TResult>(
            TResult value,
            string key,
            string? settingsKey)
        {
            if (_cachedKeySettings.TryGetValue(string.IsNullOrEmpty(settingsKey) ? key : settingsKey, out CacheKeyEntry? settings) && settings is not null)
            {
                if (settings.NoExpiration)
                {
                    this.Set(key, value);
                    return value;
                }

                var timeSpan = settings.Expiration?.GetTimeSpan();

                if (timeSpan is null)
                {
                    throw new KwfOnMemoryCacheException(Constants.CacheConfigurationKey, $"{key} doesn't have Expiration defined, either set expiration object or NoExpiration flag");
                }

                this.Set(
                    key,
                    value,
                    timeSpan.Value);
                return value;
            }

            this.Set(
                key,
                value,
                _defaultCacheExpiration);

            return value;
        }
    }
}
