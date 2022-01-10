namespace KWFAuthentication.Extensions
{
    using KWFAuthentication.Abstractions.Constants;
    using KWFAuthentication.Abstractions.Context;
    using KWFAuthentication.Implementation.Configuration;
    using KWFAuthentication.Implementation.Context;
    using KWFAuthentication.Implementation.Jwt;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KwfAuthExtensions
    {
        public static IServiceCollection AddKwfAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddBearerAuthentication(configuration);
            services.AddKwfAuthorization();
            services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

            return services;
        }

        public static IApplicationBuilder UseKwfAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
