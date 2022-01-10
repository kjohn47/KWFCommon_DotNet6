namespace KWFCommon.Extensions
{
    using KWFCommon.Implementation.DependencyInjection;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Text.Json;
    using Microsoft.AspNetCore.Builder;

    public static class AddDependenciesExtensions
    {
        public static IServiceCollection AddKWFCommon(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, IConfiguration> registerAuth,
            Action<IServiceCollection, IConfiguration, JsonSerializerOptions, bool> registerApplicationServices,
            bool isDev)
        {            
            return AddDependencies.AddKWFCommon(applicationBuilder, registerAuth, registerApplicationServices, isDev);
        }

        public static IServiceCollection AddKWFCommon(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, IConfiguration> registerAuth,
            Action<IServiceCollection, IConfiguration, JsonSerializerOptions, bool> registerApplicationServices)
        {
            return AddDependencies.AddKWFCommon(applicationBuilder, registerAuth, registerApplicationServices, false);
        }

        public static IServiceCollection AddKWFCommon(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, IConfiguration> registerAuth,
            bool isDev)
        {
            return AddDependencies.AddKWFCommon(applicationBuilder, registerAuth, null, isDev);
        }

        public static IServiceCollection AddKWFCommon(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, IConfiguration> registerAuth)
        {
            return AddDependencies.AddKWFCommon(applicationBuilder, registerAuth, null, false);
        }

        public static IServiceCollection AddKWFCommon(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, IConfiguration, JsonSerializerOptions, bool> registerApplicationServices,
            bool isDev)
        {
            return AddDependencies.AddKWFCommon(applicationBuilder, null, registerApplicationServices, isDev);
        }

        public static IServiceCollection AddKWFCommon(this WebApplicationBuilder applicationBuilder,
            Action<IServiceCollection, IConfiguration, JsonSerializerOptions, bool> registerApplicationServices)
        {
            return AddDependencies.AddKWFCommon(applicationBuilder, null, registerApplicationServices, false);
        }
    }
}
