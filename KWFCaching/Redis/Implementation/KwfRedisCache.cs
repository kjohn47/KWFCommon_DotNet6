namespace KWFCaching.Redis.Implementation
{
    using KWFCaching.Abstractions.Interfaces;
    using KWFCaching.Abstractions.Models;
    using KWFCaching.Redis.Interfaces;

    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.StackExchangeRedis;

    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class KwfRedisCache : RedisCache, IKwfRedisCache
    {
        private readonly IDictionary<string, CacheKeyEntry> _cachedKeySettings;

        private readonly TimeSpan _defaultCacheInterval;

        private static JsonSerializerOptions _jsonSerializerOptions = GetJsonOptions();

        public KwfRedisCache(KwfRedisCacheOptions options)
            : base(options?.BuildRedisCacheOptions() ?? new RedisCacheOptions())
        {
            _cachedKeySettings = options?.CachedKeySettings ?? new Dictionary<string, CacheKeyEntry>();
            _defaultCacheInterval = options?.DefaultCacheInterval?.GetTimeSpan() ?? new TimeSpan(0, 60, 0);
        }

        public Task<CachedResult<TResult>> GetCachedItemAsync<TResult>(
            string key,
            CancellationToken? cancellationToken = null)
            where TResult : class
        {
            return GetAndValidateCacheValueAsync<TResult>(key, cancellationToken);
        }

        public Task SetCachedItemAsync<TItem>(
            string key,
            TItem value,
            string? settingsKey = null,
            CancellationToken? cancellationToken = null)
            where TItem : class
        {
            return SetCacheValueAsync(value, key, settingsKey, cancellationToken);
        }

        public Task RemoveCachedItemAsync(
            string key,
            CancellationToken? cancellationToken = null)
        {
            return RemoveAsync(key, cancellationToken?? default);
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
            CachedResult<TResult> cachedValue = await GetAndValidateCacheValueAsync<TResult>(key, cancellationToken);
            if (cachedValue.CacheMiss)
            {
                var response = await fetchDataHandler(cancellationToken);
                if (fetchDataResultHandler is not null)
                {
                    var handledResponse = fetchDataResultHandler(response);
                    if (handledResponse.PreventCacheWrite)
                    {
                        cachedValue.Result = handledResponse.Result;
                        return cachedValue;
                    }
                    response = handledResponse.Result;
                }

                cachedValue.Result = await SetCacheValueAsync(response, key, settingsKey, cancellationToken);
            }

            return cachedValue;
        }

        private async Task<CachedResult<TResult>> GetAndValidateCacheValueAsync<TResult>(
            string key,
            CancellationToken? cancellationToken)
            where TResult : class
        {
            var value = await this.GetStringAsync(key, cancellationToken ?? default);
            if (!string.IsNullOrEmpty(value))
            { 
                return new CachedResult<TResult>
                {
                    Result = ParseStringToObject<TResult>(value)
                };
            }

            return new CachedResult<TResult>()
            {
                CacheMiss = true
            };
        }

        private async Task<TResult> SetCacheValueAsync<TResult>(
            TResult value,
            string key,
            string? settingsKey,
            CancellationToken? cancellationToken)
        {
            if (_cachedKeySettings.TryGetValue(string.IsNullOrEmpty(settingsKey) ? key : settingsKey, out CacheKeyEntry? settings) && settings is not null)
            {
                if (settings.NoExpiration)
                {
                    await this.SetStringAsync(key, SerializeObject(value), cancellationToken ?? default);
                    return value;
                }

                var timespan = settings.Expiration?.GetTimeSpan();

                if (timespan is null)
                {
                    throw new KwfRedisCacheException("REDISNOEXPIRATIONDEFINED", $"{key} doesn't have Expiration defined, either set expiration object or NoExpiration flag");
                }

                await this.SetStringAsync(
                    key,
                    SerializeObject(value),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = timespan.Value
                    },
                    cancellationToken ?? default);
                return value;
            }

            await this.SetStringAsync(
                key,
                SerializeObject(value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _defaultCacheInterval
                },
                cancellationToken ?? default);

            return value;
        }

        private static string SerializeObject<T>(T value)
        {
            try
            {
                return JsonSerializer.Serialize(value, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                throw new KwfRedisCacheException("REDISSERIALIZEOBJEX","An error occured while serializing object to redis", ex);
            }
        }

        private static T ParseStringToObject<T>(string value)
            where T : class
        {
            try
            {
                return JsonSerializer.Deserialize<T>(value, _jsonSerializerOptions)!;
            }
            catch (Exception ex)
            {
                throw new KwfRedisCacheException("REDISSERIALIZEOBJEX", "An error occured while serializing object to redis", ex);
            }
        }

        private static JsonSerializerOptions GetJsonOptions()
        {
            var settings = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            settings.Converters.Add(new JsonStringEnumConverter());

            return settings;
        }
    }
}
