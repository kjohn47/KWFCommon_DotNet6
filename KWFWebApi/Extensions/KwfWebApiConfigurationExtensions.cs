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
        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
           IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
           Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices,
           params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, loggerProviders, addApplicationServices, endpointConfigurations);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, loggerProviders, null, endpointConfigurations);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, loggerProviders, addApplicationServices);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
            IEnumerable<KwfLoggerProviderBuilder> loggerProviders)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, loggerProviders, null);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
           Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices,
           params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, null, addApplicationServices, endpointConfigurations);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
            params IEndpointConfiguration[] endpointConfigurations)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, null, null, endpointConfigurations);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]> addApplicationServices)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, null, addApplicationServices);
        }

        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder)
        {
            KwfWebApiConfiguration.RunKwfApplication(applicationBuilder, null, null);
        }
    }
}
