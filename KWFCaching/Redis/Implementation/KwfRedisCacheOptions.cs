namespace KWFCaching.Redis.Implementation
{
    using KWFCaching.Abstractions.Models;

    using Microsoft.Extensions.Caching.StackExchangeRedis;

    using StackExchange.Redis;

    using System.Security.Authentication;

    public class KwfRedisCacheOptions
    {
        private RedisCacheOptions? _redisCacheOptions;
        private ConfigurationOptions? _redisConfiguration;

        public CacheTimeSettings DefaultCacheInterval { get; set; } = new CacheTimeSettings
        {
            Minutes = 60
        };

        public IEnumerable<KwfRedisEndpoint>? Endpoints  { get; set; }
        public IDictionary<string, CacheKeyEntry>? CachedKeySettings { get; set; }
        public int DatabaseId { get; set; } = 0;
        public int ResponseTimeoutMs { get; set; } = 5000;
        public int ConnectionTimeoutMs { get; set; } = 5000;
        public string? Name { get; set; }
        public string? ServiceName { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public bool SslEnable { get; set; } = false;
        public string? SslHost { get; set; }
        public IEnumerable<SslProtocols>? SslProtocols { get; set; }

        public RedisCacheOptions BuildRedisCacheOptions()
        {
            if (_redisCacheOptions is null)
            {
                _redisCacheOptions = new RedisCacheOptions
                {
                    ConfigurationOptions = GetRedisConfiguration()
                };
            }

            return _redisCacheOptions;
        }

        private ConfigurationOptions GetRedisConfiguration()
        {
            if (Endpoints is null || !Endpoints.Any())
            {
                throw new KwfRedisCacheException("REDISMISSINGENDPOINTS", "You have to define endpoints for your redis connection");
            }

            _redisConfiguration = new ConfigurationOptions
            {
                DefaultDatabase = DatabaseId,
                ConnectTimeout = ConnectionTimeoutMs,
                SyncTimeout = ResponseTimeoutMs,
                AsyncTimeout = ResponseTimeoutMs
            };

            SetRedisEndpoints();
            SetRedisUser();
            SetRedisPassword();
            SetRedisClientName();
            SetRedisServiceName();
            SetRedisSSl();

            return _redisConfiguration;
        }

        private void SetRedisEndpoints()
        {
            foreach (var endpoint in Endpoints!)
            {
                if (string.IsNullOrEmpty(endpoint.Url))
                {
                    throw new KwfRedisCacheException("REDISMISSINGENDPOINTURL", "You have to define url on all endpoints for your redis connection");
                }

                _redisConfiguration!.EndPoints.Add(endpoint.Url, endpoint.Port);
            }
        }

        private void SetRedisUser()
        {
            if (!string.IsNullOrEmpty(Password))
            {
                _redisConfiguration!.User = User;
            }
        }

        private void SetRedisPassword()
        {
            if (!string.IsNullOrEmpty(Password))
            {
                _redisConfiguration!.Password = Password;
            }
        }

        private void SetRedisClientName()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                _redisConfiguration!.ClientName = Name;
            }
        }

        private void SetRedisServiceName()
        {
            if (!string.IsNullOrEmpty(ServiceName))
            {
                _redisConfiguration!.ServiceName = ServiceName;
            }
        }

        private void SetRedisSSl()
        {
            if (SslEnable)
            {
                _redisConfiguration!.Ssl = true;
                if (SslProtocols is not null && SslProtocols.Any())
                {
                    _redisConfiguration!.SslProtocols = SslProtocols.Count() > 1 
                                                        ? SslProtocols.Aggregate((prev, next) => prev | next)
                                                        : SslProtocols.First();
                }

                if (!string.IsNullOrEmpty(SslHost))
                {
                    _redisConfiguration!.SslHost = SslHost;
                }
            }
        }
    }
}
