﻿namespace KWFCommon.Implementation.ServiceConfiguration
{
    using System;
    using System.Text.Json;

    using KWFCommon.Implementation.Configuration;
    using KWFCommon.Abstractions.Constants;
    using KWFCommon.Implementation.Exception;
    using KWFCommon.Implementation.Json;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;

    public static class ConfigureServices
    {
        public static IApplicationBuilder UseKWFCommon(
            this WebApplication webApp,
            Action<IApplicationBuilder>? configureAuth,
            Action<IApplicationBuilder, IConfiguration, JsonSerializerOptions, bool>? configureApplicationServices,
            Action<IEndpointRouteBuilder, IConfiguration, JsonSerializerOptions>? configureEndpoints,
            bool isDev)
        {
            IApplicationBuilder app = webApp;
            if (app.ApplicationServices.GetService(typeof(AppConfiguration)) is not AppConfiguration apiConfig)
            {
                throw new ArgumentNullException(ApiConstants.AppConfiguration_Key, "Missing App Configuration");
            }

            var serializerOpt = (app.ApplicationServices.GetService(typeof(KWFJsonConfiguration)) is KWFJsonConfiguration jsonOptions
                                    ? jsonOptions
                                    : new KWFJsonConfiguration(isDev))
                                .GetJsonSerializerOptions();
            if (apiConfig.LocalizationConfiguration is not null) app.UseLocalization(apiConfig.LocalizationConfiguration);
            
            app.UseExceptionHandlerMidleware(serializerOpt, isDev);
            
            if (apiConfig.KestrelConfiguration?.HasHttpsAvailable ?? false) app.UseHttpsRedirection();

            app.UseStatusCodeHandler(serializerOpt, isDev);
            app.UseCorsConfigurator(apiConfig.CorsConfiguration, isDev);

            if (apiConfig.SwaggerSettings is not null)
            {
                app.UseSwagger(apiConfig.SwaggerSettings);
            }

            if (configureAuth is not null) configureAuth(app);
            if (configureApplicationServices is not null) configureApplicationServices(app, webApp.Configuration, serializerOpt, isDev);
            if (configureEndpoints is not null) configureEndpoints(webApp, webApp.Configuration, serializerOpt);

            return app;
        }
    }
}
