namespace KWFCommon.Implementation.ServiceConfiguration
{
    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;

    public static class SwaggerConfigurator
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, OpenApiSettings configuration)
        {
            if (!configuration.UseDocumentation)
            {
                return app;
            }

            app.UseSwagger();

            if (!configuration.UseUI)
            {
                return app;
            }

            app.UseSwaggerUI(option => option.SwaggerEndpoint($"/swagger/{configuration.ApiName}/swagger.json", configuration.ApiName));

            return app;
        }
    }
}
