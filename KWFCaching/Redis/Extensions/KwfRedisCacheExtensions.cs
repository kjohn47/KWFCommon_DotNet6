namespace KWFCaching.Redis.Extensions
{
    using KWFCaching.Redis.Implementation;
    using KWFCaching.Redis.Interfaces;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KwfRedisCacheExtensions
    {
        public static IServiceCollection AddKwfRedisCache(
            this IServiceCollection services,
            IConfiguration configuration,
            string? customConfigurationKey = null)
        {
            return services.AddKwfRedisCache(configuration.GetRedisCacheOptions(customConfigurationKey));
        }

        public static IServiceCollection AddKwfRedisCache(
            this IServiceCollection services,
            KwfRedisCacheOptions cacheSettings)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (cacheSettings == null)
            {
                throw new ArgumentNullException(nameof(cacheSettings));
            }

            services.TryAddSingleton<IKwfRedisCache>(services => new KwfRedisCache(cacheSettings));
            return services;
        }

        public static KwfRedisCacheOptions GetRedisCacheOptions(
            this IConfiguration configuration,
            string? customConfigurationKey = null)
        {
            return configuration.GetSection(customConfigurationKey ?? "KwfRedisCacheConfiguration").Get<KwfRedisCacheOptions>() ?? new KwfRedisCacheOptions();
        }
    }
}
