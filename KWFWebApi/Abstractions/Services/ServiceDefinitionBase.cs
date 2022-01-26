namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Service Definition Base class that implements IServiceDefinition
    /// Override Method AddServices to add your services to App
    /// Override Method ConfigureServices to configure your services
    /// </summary>
    public abstract class ServiceDefinitionBase : IServiceDefinition
    {
        /// <summary>
        /// The Is Development Environment Flag
        /// </summary>
        protected bool IsDev { get; private set; }

        /// <summary>
        /// The App Configuration
        /// </summary>
        protected IConfiguration Configuration { get; private set; }

        public ServiceDefinitionBase(IConfiguration configuration, bool isDev)
        {
            Configuration = configuration;
            IsDev = isDev;
        }

        public virtual void AddServices(IServiceCollection services)
        {
        }

        public virtual void ConfigureServices(IApplicationBuilder app)
        {
        }
    }
}
