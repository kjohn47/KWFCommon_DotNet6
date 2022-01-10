namespace KWFWebApi.Abstractions.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    public interface IKwfEndpointInitialize
    {
        IKwfEndpointBuilder InitializeEndpoint(string endpoint);
        IKwfEndpointBuilder InitializeEndpoint(string endpoint, bool requireAuthorization);
        IKwfEndpointBuilder InitializeEndpoint(string endpoint, params string[] globalAuthorizationPolicy);
        IKwfEndpointBuilder InitializeEndpoint(string endpoint, PoliciesEnum globalAuthorizationPolicy);
    }
}
