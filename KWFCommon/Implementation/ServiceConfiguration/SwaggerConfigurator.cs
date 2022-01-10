namespace KWFCommon.Implementation.ServiceConfiguration
{
    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;

    public static class SwaggerConfigurator
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, SwaggerSettings configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(option => option.SwaggerEndpoint($"/swagger/{configuration.ApiName}/swagger.json", configuration.ApiName));

            return app;
        }
    }
}
