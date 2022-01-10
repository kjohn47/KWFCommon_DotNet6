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

    public static class KwfWebApiConfiguration
    {
        public static void RunKwfApplication(this WebApplicationBuilder applicationBuilder,
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
                    isDev)
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
                    isDev)
                .Run();
        }

        private static WebApplication BuildKwfServices(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, JsonSerializerOptions> applicationServices,
            IEnumerable<KwfLoggerProviderBuilder>? loggerProviders,
            bool isDev)
        {
            applicationBuilder.Services.AddLogging(applicationBuilder.Configuration, loggerProviders, isDev);
            applicationBuilder.AddKWFCommon(
                (s, cfg) => s.AddKwfAuth(cfg),
                (s, cfg, jsonOpt, isDev) => applicationServices(s, jsonOpt),
                isDev);

            return applicationBuilder.Build();
        }

        private static WebApplication UseKwfConfiguration(this WebApplication app,
            Action<IApplicationBuilder> configureApplicationServices,
            Action<IEndpointRouteBuilder, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            app.UseKWFCommon(
                a => a.UseKwfAuth(),
                (a, cfg, jsonCfg, dev) => {
                    a.UseLogging(cfg, dev);
                    configureApplicationServices(a);
                },
                (a, cfg, jsonOpt) => configureEndpoints(a, jsonOpt),
                isDev);

            return app;
        }
    }
}
