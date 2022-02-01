namespace KWFWebApi.Abstractions.Services
{
    using KWFWebApi.Abstractions.Endpoint;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Endpoint Configuration Base class that implements IEndpointConfiguration
    /// Override Method InitializeRoute to custom your main route and global authorization - default name of parent class without authorization
    /// Override Method ConfigureEndpoints to create your endpoints (Required)
    /// </summary>
    public abstract class EndpointConfigurationBase : IEndpointConfiguration
    {
        public virtual IKwfEndpointBuilder InitializeRoute(
            IKwfEndpointInitialize builder,
            IConfiguration configuration)
        {
            return builder.InitializeEndpoint(this.GetType().Name);
        }

        public abstract void ConfigureEndpoints(
            IKwfEndpointBuilder builder,
            IConfiguration configuration);
    }
}
