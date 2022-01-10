namespace KWFAuthentication.Implementation.Jwt
{
    using KWFAuthentication.Abstractions.Constants;

    using Microsoft.Extensions.DependencyInjection;

    using System.Security.Claims;

    public static class AuthorizationInjector
    {
        public static IServiceCollection AddKwfAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Administrator, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role,
                        Policies.Administrator);
                });
                options.AddPolicy(Policies.User, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role,
                        Policies.User,
                        Policies.Administrator);
                });
                options.AddPolicy(Policies.Anonymous, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role,
                        Policies.User,
                        Policies.Administrator,
                        Policies.Anonymous);
                });
            });

            return services;
        }
    }
}
