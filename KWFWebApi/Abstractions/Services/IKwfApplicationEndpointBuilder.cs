namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationEndpointBuilder : IKwfApplicationRun
    {
        IKwfApplicationEndpointBuilder AddEndpointDefinition(IEndpointConfiguration endpointConfiguration);
    }
}
