namespace KWFCommon.Extensions
{
    using System;
    using System.Text.Json;

    using KWFCommon.Implementation.ServiceConfiguration;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;

    public static class ConfigureServicesExtensions
    {
        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, configureApplicationServices, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, configureApplicationServices, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureMiddlewares, configureApplicationServices, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureMiddlewares, configureApplicationServices, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, null, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, null, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, configureApplicationServices, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, configureApplicationServices, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, null, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureMiddlewares, null, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureMiddlewares, null, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureMiddlewares, null, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureMiddlewares, configureApplicationServices, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureMiddlewares,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureMiddlewares, configureApplicationServices, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, null, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, null, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, configureApplicationServices, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, configureApplicationServices, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, null, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, null, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, null, null, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, null, null, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, null, configureApplicationServices, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IServiceProvider, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, null, configureApplicationServices, null, false);
        }
    }
}
