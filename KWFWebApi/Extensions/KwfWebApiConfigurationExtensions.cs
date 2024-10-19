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
            IEnumerable<Type> middlewareTypes,
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
                middlewareTypes,
                addApplicationServices,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            IEnumerable<Type> middlewareTypes,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                middlewareTypes,
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            IEnumerable<Type> middlewareTypes,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                middlewareTypes,
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            IEnumerable<Type> middlewareTypes)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                loggerProviders,
                middlewareTypes,
                null);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<Type> middlewareTypes,
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
                middlewareTypes,
                addApplicationServices,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<Type> middlewareTypes,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                middlewareTypes,
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<Type> middlewareTypes,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                middlewareTypes,
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            IEnumerable<Type> middlewareTypes)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication,
                null,
                middlewareTypes,
                null);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            IEnumerable<Type> middlewareTypes,
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
                middlewareTypes,
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
            IEnumerable<Type> middlewareTypes,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders,
                middlewareTypes,
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
            IEnumerable<Type> middlewareTypes,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders,
                middlewareTypes,
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            IEnumerable<Type> middlewareTypes)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                loggerProviders,
                middlewareTypes,
                null);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<Type> middlewareTypes,
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
                middlewareTypes,
                addApplicationServices,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<Type> middlewareTypes,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null,
                middlewareTypes,
                null,
                endpointConfigurations);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<Type> middlewareTypes,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null,
                middlewareTypes,
                addApplicationServices);
        }

        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            IEnumerable<Type> middlewareTypes)
        {
            KwfWebApiConfiguration.RunKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication,
                null,
                middlewareTypes,
                null);
        }

        //STOP

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
                null,
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
                null,
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
                null,
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
                null, 
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
                null,
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
                null, 
                null);
        }
    }
}
