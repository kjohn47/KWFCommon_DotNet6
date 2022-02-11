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
            IEnumerable<Type>? middlewares,
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
                .UseKwfConfiguration(
                    (app) =>
                    {
                        if (middlewares is not null)
                        {
                            foreach(var middleware in middlewares)
                            {
                                if (middleware.IsAbstract || middleware.IsInterface || !typeof(KwfMiddlewareBase).IsAssignableFrom(middleware))
                                {
                                    throw new InvalidOperationException($"Invalid middleware type {middleware.Name} for application");
                                }

                                app.UseMiddleware(middleware);
                            }
                        }
                    },
                    (serviceProvider) =>
                    {
                        if (applicationServices is not null)
                        {
                            foreach (var serviceDef in applicationServices)
                            {
                                serviceDef.ConfigureServices(serviceProvider);
                            }
                        }
                    },
                    (app, jsonOpt) =>
                    {
                        if (endpointConfigurations is not null)
                        {
                            var handler = new KwfEndpointHandler(app, jsonOpt);
                            foreach (var endpointCfg in endpointConfigurations)
                            {
                                var routeBuilder = KwfEndpointBuilder.CreateEndpointBuilder(app);
                                endpointCfg.InitializeRoute(routeBuilder, applicationBuilder.Configuration);
                                if (string.IsNullOrEmpty(routeBuilder?.BaseUrl))
                                {
                                    throw new ArgumentNullException(nameof(KwfEndpointBuilder), "BaseUrl must be set for endpoint");
                                }

                                endpointCfg.ConfigureEndpoints(routeBuilder, handler, applicationBuilder.Configuration);
                                routeBuilder.Build();
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
            Action<IApplicationBuilder> addMiddlewares,
            Action<IServiceProvider> configureApplicationServices,
            Action<IEndpointRouteBuilder, JsonSerializerOptions> configureEndpoints,
            bool enableAuthentication,
            bool isDev,
            string? customLoggingConfigurationKey = null)
        {
            app.UseKWFCommon(
                a => { if (enableAuthentication) a.UseKwfAuth(); },
                (a, cfg, jsonCfg, dev) => {
                    a.UseLogging(cfg, dev, customLoggingConfigurationKey);
                    addMiddlewares(a);
                },
                (sp, cfg, jsonCfg, dev) => configureApplicationServices(sp),
                (a, cfg, jsonOpt) => configureEndpoints(a, jsonOpt),
                isDev);

            return app;
        }
    }
}
