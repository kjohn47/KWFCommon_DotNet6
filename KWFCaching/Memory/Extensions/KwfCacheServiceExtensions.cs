namespace KWFCaching.Memory.Extensions
{
    using System;

    using KWFCaching.Memory.Implementation;
    using KWFCaching.Memory.Interfaces;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KwfCacheServiceExtensions
    {
        public static IServiceCollection AddKwfCacheOnMemory(
            this IServiceCollection services,
            IConfiguration configuration,
            string? customConfigurationKey = null)
        {
            KwfCacheConfiguration cacheSettings = configuration.GetSection(customConfigurationKey ?? Constants.CacheConfigurationKey).Get<KwfCacheConfiguration>() ?? new KwfCacheConfiguration();
            return AddKwfCacheOnMemory(services, cacheSettings);
        }

        public static IServiceCollection AddKwfCacheOnMemory(
            this IServiceCollection services,
            KwfCacheConfiguration cacheSettings)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (cacheSettings == null)
            {
                throw new ArgumentNullException(nameof(cacheSettings));
            }

            services.AddOptions();

            services.TryAddSingleton<IKwfCacheOnMemory, KwfCacheOnMemory>();

            services.Configure<KwfCacheOptions>(x =>
            {
                x.ExpirationScanFrequency = cacheSettings.CleanupInterval?.GetTimeSpan() ?? new TimeSpan(0, 30, 0);

                if (cacheSettings.CacheSizeSettings is not null)
                {
                    if (cacheSettings.CacheSizeSettings.MaxSize.HasValue)
                    {
                        x.SizeLimit = cacheSettings.CacheSizeSettings.MaxSize.Value;
                    }

                    if (cacheSettings.CacheSizeSettings.CompactionPercentage.HasValue)
                    {
                        x.SizeLimit = cacheSettings.CacheSizeSettings.CompactionPercentage.Value;
                    }
                }

                x.CachedKeySettings = cacheSettings.CacheKeySettings;
                x.DefaultCacheExpiration = cacheSettings.DefaultCacheExpiration?.GetTimeSpan();
            });

            return services;
        }
    }
}
