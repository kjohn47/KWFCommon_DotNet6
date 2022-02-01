namespace KWFWebApi.Extensions
{
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Implementation.Logging;
    using KWFWebApi.Implementation.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;

    using System;

    public static class KwfWebApiConfigurationExtensions
    {
        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                addApplicationServices,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                null);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                addApplicationServices,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                null);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey, 
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders,
                addApplicationServices, 
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders,
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders, 
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders,
                null);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null,
                addApplicationServices, 
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null, 
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null, 
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null, 
                null);
        }
    }
}
