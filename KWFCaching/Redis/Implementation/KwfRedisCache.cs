namespace KWFCaching.Redis.Implementation
{
    using KWFCaching.Abstractions.Interfaces;
    using KWFCaching.Abstractions.Models;
    using KWFCaching.Redis.Interfaces;
    using KWFJson.Configuration;

    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.StackExchangeRedis;

    using StackExchange.Redis;

    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class KwfRedisCache : IKwfRedisCache, IDisposable
    {
        private bool _disposed;

        private readonly RedisCache _redisCache;

        private IConnectionMultiplexer? _connectionMultiplexer;

        private IDatabase? _db;

        private readonly IDictionary<string, CacheKeyEntry> _cachedKeySettings;

        private readonly TimeSpan _defaultCacheInterval;

        private readonly RedisCacheOptions? _redisConfiguration;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        private static readonly JsonSerializerOptions _jsonSerializerOptions = GetJsonOptions();

        public IDatabase RedisDatabase => _db ?? GetRedisDatabaseAsync().GetAwaiter().GetResult();

        public KwfRedisCache(KwfRedisCacheOptions options)
        {
            _cachedKeySettings = options?.CachedKeySettings ?? new Dictionary<string, CacheKeyEntry>();
            _defaultCacheInterval = options?.DefaultCacheInterval?.GetTimeSpan() ?? new TimeSpan(0, 60, 0);
            _redisConfiguration = options?.BuildRedisCacheOptions();
            if (_redisConfiguration?.ConfigurationOptions is null)
            {
                throw new KwfRedisCacheException("REDISMISSINGCONFIG", "Could not connect to redis cache server : missing configuration");
            }

            _redisConfiguration.ConnectionMultiplexerFactory = async () =>
            {
                if (_db is null)
                {
                    await ConnectAsync();
                }

                return _connectionMultiplexer;
            };

            _redisCache = new RedisCache(_redisConfiguration);
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

        public byte[] Get(string key) => _redisCache.Get(key);

        public Task<byte[]> GetAsync(string key, CancellationToken token = default) => _redisCache.GetAsync(key, token);

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => _redisCache.Set(key, value, options);

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
            => _redisCache.SetAsync(key, value, options, token);

        public void Refresh(string key) => _redisCache.Refresh(key);

        public Task RefreshAsync(string key, CancellationToken token = default) => _redisCache.RefreshAsync(key, token);

        public void Remove(string key) => _redisCache.Remove(key);

        public Task RemoveAsync(string key, CancellationToken token = default) => _redisCache.RemoveAsync(key, token);

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _connectionMultiplexer?.Close();
                _redisCache?.Dispose();
            }
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

        private async Task ConnectAsync()
        {
            if (_disposed)
            {
                throw new KwfRedisCacheException("REDISDISPOS", "Kwf Redis cache was disposed and cannot initialize connection");
            }

            if (_db is not null)
            {
                return;
            }

            _connectionLock.Wait();
            try
            {
                if (_connectionMultiplexer is null)
                {
                    _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_redisConfiguration!.ConfigurationOptions);
                }

                _db = _connectionMultiplexer!.GetDatabase();
            }
            catch (Exception ex)
            {
                throw new KwfRedisCacheException("KWFREDISCONFAIL", "Failed to connect to Redis server", ex);
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private async Task<IDatabase> GetRedisDatabaseAsync()
        {
            if (_db is null)
            {
                await ConnectAsync();
            }

            return _db!;
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
            return new KWFJsonConfiguration().GetJsonSerializerOptions();
        }
    }
}
