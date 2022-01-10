namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationEndpointsBuilder: IKwfApplicationEndpointBuilder
    {
        IKwfApplicationRun AddEndpointDefinitionRange(params IEndpointConfiguration[] endpointConfiguration);
    }
}
