namespace KWFWebApi.Implementation.Services
{
    using System.Text.Json;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Routing;

    using KWFWebApi.Implementation.Logging;

    using KWFCommon.Extensions;
    using KWFAuthentication.Extensions;
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Implementation.Endpoint;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.AspNetCore.Http;

    public static class KwfWebApiConfiguration
    {
        public static void RunKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            bool enableAuthentication,
            IEnumerable<KwfLoggerProviderBuilder>? loggerProviders,
            Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]>? addApplicationServices,
            params IEndpointConfiguration[]? endpointConfigurations)
        {
            var isDev = applicationBuilder.Environment.IsDevelopment();
            IServiceDefinition[]? applicationServices = null;
            
            if (addApplicationServices is not null)
            {
                applicationServices = addApplicationServices((applicationBuilder.Configuration, isDev));
            }

            applicationBuilder
                .BuildKwfServices((svc, jsonOpt) => 
                    { 
                        if (applicationServices is not null)
                        {
                            foreach (var serviceProvider in applicationServices)
                            {
                                serviceProvider.AddServices(svc);
                            }
                        }
                    },
                    loggerProviders,
                    enableAuthentication,
                    isDev,
                    customAppConfigurationKey,
                    customBearerConfigurationKey,
                    customLoggingConfigurationKey)
                .UseKwfConfiguration((app) =>
                    {
                        if (applicationServices is not null)
                        {
                            foreach (var serviceProvider in applicationServices)
                            {
                                serviceProvider.ConfigureServices(app);
                            }
                        }
                    },
                    (app, jsonOpt) =>
                    {
                        if (endpointConfigurations is not null)
                        {
                            foreach(var endpointCfg in endpointConfigurations)
                            {
                                var routeBuilder = KwfEndpointBuilder.CreateEndpointBuilder(app, jsonOpt);
                                var endpointBuilder = endpointCfg.InitializeRoute(routeBuilder, applicationBuilder.Configuration);
                                if (string.IsNullOrEmpty((endpointBuilder as KwfEndpointBuilder)?.BaseUrl))
                                {
                                    throw new ArgumentNullException(nameof(KwfEndpointBuilder), "BaseUrl must be set for endpoint");
                                }

                                endpointCfg.ConfigureEndpoints(endpointBuilder, applicationBuilder.Configuration);
                            }
                        }
                    },
                    enableAuthentication,
                    isDev,
                    customLoggingConfigurationKey)
                .Run();
        }

        private static WebApplication BuildKwfServices(
            this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, JsonSerializerOptions> applicationServices,
            IEnumerable<KwfLoggerProviderBuilder>? loggerProviders,
            bool enableAuthentication,
            bool isDev,
            string? customAppConfigurationKey = null,
            string? customBearerConfigurationKey = null,
            string? customLoggingConfigurationKey = null)
        {
            applicationBuilder.Services.AddLogging(applicationBuilder.Configuration, loggerProviders, isDev, customLoggingConfigurationKey);
            applicationBuilder.AddKWFCommon(
                customAppConfigurationKey,
                (s, cfg) => { if (enableAuthentication) s.AddKwfAuth(cfg, customBearerConfigurationKey); },
                (s, cfg, jsonOpt, isDev) => {
                        s.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                        applicationServices(s, jsonOpt);
                    },
                isDev);

            return applicationBuilder.Build();
        }

        private static WebApplication UseKwfConfiguration(
            this WebApplication app,
            Action<IApplicationBuilder> configureApplicationServices,
            Action<IEndpointRouteBuilder, JsonSerializerOptions> configureEndpoints,
            bool enableAuthentication,
            bool isDev,
            string? customLoggingConfigurationKey = null)
        {
            app.UseKWFCommon(
                a => { if (enableAuthentication) a.UseKwfAuth(); },
                (a, cfg, jsonCfg, dev) => {
                    a.UseLogging(cfg, dev, customLoggingConfigurationKey);
                    configureApplicationServices(a);
                },
                (a, cfg, jsonOpt) => configureEndpoints(a, jsonOpt),
                isDev);

            return app;
        }
    }
}
