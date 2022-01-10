namespace KWFWebApi.Abstractions.Services
{
    using KWFWebApi.Abstractions.Endpoint;

    using Microsoft.Extensions.Configuration;

    public interface IEndpointConfiguration
    {
        IKwfEndpointBuilder InitializeRoute(IKwfEndpointInitialize builder, IConfiguration configuration);
        void ConfigureEndpoints(IKwfEndpointBuilder builder, IConfiguration configuration);
    }
}
