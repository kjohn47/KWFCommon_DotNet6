namespace KWFWebApi.Implementation.Logging
{
    using KWFCommon.Abstractions.Constants;

    using KWFWebApi.Abstractions.Constants;
    using KWFWebApi.Abstractions.Logging;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.HttpLogging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class LoggingConfigurator
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration, bool isDev = false)
        {
            return AddLoggingInternal(services, configuration, null, isDev);
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration, IEnumerable<KwfLoggerProviderBuilder>? additionalProviders, bool isDev = false)
        {
            return AddLoggingInternal(services, configuration, additionalProviders, isDev);
        }

        private static IServiceCollection AddLoggingInternal(IServiceCollection services, IConfiguration configuration, IEnumerable<KwfLoggerProviderBuilder>? additionalProviders, bool isDev)
        {
            var config = configuration.GetSection(LoggingConstants.Configuration_Key).Get<LoggingConfiguration>();

            if (!isDev && !config.EnableApiLogs)
            {
                services.AddLogging(l =>
                {
                    l.ClearProviders();
                });

                return services;
            }

            services.AddLogging(l =>
            {
                l.ClearProviders();
                l.AddConfiguration(configuration.GetSection(ApiConstants.Logging_Key));
                if (config.Providers is not null)
                {
                    if (config.Providers.Any(x => x.Equals(nameof(LoggingProviderEnum.Console)))) l.AddConsole();
                    if (config.Providers.Any(x => x.Equals(nameof(LoggingProviderEnum.Debug)))) l.AddDebug();
                    if (config.Providers.Any(x => x.Equals(nameof(LoggingProviderEnum.Event)))) l.AddEventLog();
                    if (config.Providers.Any(x => x.Equals(nameof(LoggingProviderEnum.EventSource)))) l.AddEventSourceLogger();

                    if (additionalProviders is not null)
                    {
                        foreach (var provider in additionalProviders)
                        {
                            if (config.Providers.Any(x => x.Equals(provider.ProviderName))) provider.AddProvider(l, configuration);
                        }
                    }
                }
            });

            if ((isDev || config.EnableApiLogs) && config.EnableHttpLogs)
            {
                services.AddHttpLogging(o =>
                {
                    o.LoggingFields = HttpLoggingFields.All;
                });
            }

            return services;
        }

        public static IApplicationBuilder UseLogging(this IApplicationBuilder app, IConfiguration configuration, bool isDev = false)
        {
            var config = configuration.GetSection(LoggingConstants.Configuration_Key).Get<LoggingConfiguration>();

            if (isDev || config.EnableHttpLogs)
            {
                app.UseHttpLogging();
            }

            return app;
        }
    }
}
