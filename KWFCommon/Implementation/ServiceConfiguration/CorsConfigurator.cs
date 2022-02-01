namespace KWFCommon.Implementation.ServiceConfiguration
{
    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;

    public static class CorsConfigurator
    {
        public static IApplicationBuilder UseCorsConfigurator(
            this IApplicationBuilder app,
            CorsConfiguration? configuration,
            bool isDev = false)
        {
            if (isDev)
            {
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(CorsConfiguration), "Missing Cors Configuration");
            }

            app.UseCors(x =>
            {
                if (configuration.AllowAnyOrigin)
                {
                    x.AllowAnyOrigin();
                }
                else
                {
                    if (configuration.AllowedOrigins != null && configuration.AllowedOrigins.Length > 0)
                    {
                        x.WithOrigins(configuration.AllowedOrigins);
                    }
                }

                if (configuration.AllowAnyHeader)
                {
                    x.AllowAnyHeader();
                }
                else
                {
                    if (configuration.AllowedHeaders != null && configuration.AllowedHeaders.Length > 0)
                    {
                        x.WithHeaders(configuration.AllowedHeaders);
                    }
                }

                if (configuration.AllowAnyMethod)
                {
                    x.AllowAnyMethod();
                }
                else
                {
                    if (configuration.AllowedMethods != null && configuration.AllowedMethods.Length > 0)
                    {
                        x.WithMethods(configuration.AllowedMethods);
                    }
                }
            });

            return app;
        }
    }
}
