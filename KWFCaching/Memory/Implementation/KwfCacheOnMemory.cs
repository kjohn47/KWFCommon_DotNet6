namespace KWFCaching.Memory.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using KWFCaching.Memory.Interfaces;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class KwfCacheOnMemory : MemoryCache, IKwfCacheOnMemory
    {
        private readonly IDictionary<string, CacheKeyEntry> _cachedKeySettings;

        private const int DefaultCacheIntervalMinutes = 60;

        public KwfCacheOnMemory(Microsoft.Extensions.Options.IOptions<KwfCacheOptions> options) : base(options)
        {
            if (options != null && options.Value != null && options.Value.CachedKeySettings != null)
            {
                _cachedKeySettings = options.Value.CachedKeySettings;
            }
            else
            {
                _cachedKeySettings = new Dictionary<string, CacheKeyEntry>();
            }
        }

        public KwfCacheOnMemory(Microsoft.Extensions.Options.IOptions<KwfCacheOptions> options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            if (options != null && options.Value != null && options.Value.CachedKeySettings != null)
            {
                _cachedKeySettings = options.Value.CachedKeySettings;
            }
            else
            {
                _cachedKeySettings = new Dictionary<string, CacheKeyEntry>();
            }
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

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, null, null);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, null, settingsKey);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, null, null);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, null, settingsKey);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, fetchDataResultHandler, null);
        }

        public Task<CachedResult<TResult>> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class
        {
            return GetItemFromCacheAsync(key, fetchDataHandler, fetchDataResultHandler, settingsKey);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, fetchDataResultHandler, null);
        }

        public Task<TResult> GetOrInsertCachedItemAsync<TResult>(string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler) where TResult : class
        {
            return GetHandledItemFromCacheAsync(key, fetchDataHandler, resultHandler, fetchDataResultHandler, settingsKey);
        }

        private async Task<TResult> GetHandledItemFromCacheAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<CachedResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>>? fetchDataResultHandler = null, string? settingsKey = null)
            where TResult : class
        {
            return resultHandler(await GetItemFromCacheAsync(key, fetchDataHandler, fetchDataResultHandler, settingsKey));
        }

        private async Task<CachedResult<TResult>> GetItemFromCacheAsync<TResult>(string key, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>>? fetchDataResultHandler = null, string? settingsKey = null)
            where TResult : class
        {
            CachedResult<TResult> cachedValue = GetAndValidateCacheValue<TResult>(key);
            if (cachedValue.CacheMiss)
            {
                var response = await fetchDataHandler();
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

        private TResult SetCacheValue<TResult>(TResult value, string key, string? settingsKey)
        {
            if (_cachedKeySettings.TryGetValue(string.IsNullOrEmpty(settingsKey) ? key : settingsKey, out CacheKeyEntry? settings) && settings is not null)
            {
                if (settings.NoExpiration)
                {
                    this.Set(key, value);
                    return value;
                }

                var hours = settings.Expiration?.Hours ?? 0;
                var minutes = settings.Expiration?.Minutes ?? 0;
                var seconds = settings.Expiration?.Seconds ?? 0;

                if (hours == 0 && minutes == 0 && seconds == 0)
                {
                    throw new KwfOnMemoryCacheException(Constants.CacheConfigurationKey, $"{key} doesn't have Expiration defined, either set expiration object or NoExpiration flag");
                }

                this.Set(
                    key,
                    value,
                    DateTime.Now.AddHours(hours)
                                .AddMinutes(minutes)
                                .AddSeconds(seconds));
                return value;
            }

            this.Set(
                key,
                value,
                DateTime.Now.AddMinutes(DefaultCacheIntervalMinutes));

            return value;
        }
    }
}
