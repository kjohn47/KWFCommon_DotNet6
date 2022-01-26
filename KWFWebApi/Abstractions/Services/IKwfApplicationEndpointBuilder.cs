namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationEndpointBuilder : IKwfApplicationRun
    {
        /// <summary>
        /// Add Instance of class implementing IEndpointConfiguration or EndpointConfigurationBase
        /// </summary>
        /// <param name="endpointConfiguration">The IEndpointConfiguration</param>
        /// <returns>IKwfApplicationEndpointBuilder</returns>
        IKwfApplicationEndpointBuilder AddEndpointDefinition(IEndpointConfiguration endpointConfiguration);
    }
}
