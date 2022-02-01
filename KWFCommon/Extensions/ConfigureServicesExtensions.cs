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
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureApplicationServices, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureApplicationServices, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureApplicationServices, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureApplicationServices, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureApplicationServices, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, configureApplicationServices, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder> configureAuth)
        {
            return ConfigureServices.UseKWFCommon(webApp, configureAuth, null, null, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, null, configureEndpoints, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions> configureEndpoints)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, null, configureEndpoints, false);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices,
            bool isDev)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureApplicationServices, null, isDev);
        }

        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool> configureApplicationServices)
        {
            return ConfigureServices.UseKWFCommon(webApp, null, configureApplicationServices, null, false);
        }
    }
}
