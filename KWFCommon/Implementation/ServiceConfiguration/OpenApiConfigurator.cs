namespace KWFCommon.Implementation.ServiceConfiguration
{
    using KWFCommon.Implementation.Configuration;
    using KWFOpenApi.Html.DependencyInjection;

    using Microsoft.AspNetCore.Builder;

    public static class OpenApiConfigurator
    {
        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app, OpenApiSettings configuration)
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
            app.UseKwfOpenApiUI();
            return app;
        }
    }
}
