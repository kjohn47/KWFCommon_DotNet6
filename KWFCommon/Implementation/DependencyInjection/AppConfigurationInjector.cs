namespace KWFCommon.Implementation.DependencyInjection
{
    using KWFCommon.Implementation.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class AppConfigurationInjector
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, AppConfiguration configuration)
        {
            services.TryAddSingleton(configuration);
            return services;
        }
    }
}
