namespace KWFWebApi.Abstractions.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    public interface IKwfEndpointInitialize
    {
        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <returns>IKwfEndpointBuilder</returns>
        IKwfEndpointBuilder InitializeEndpoint(string endpoint);

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <param name="requireAuthorization">The global Require authorized user flag</param>
        /// <returns></returns>
        IKwfEndpointBuilder InitializeEndpoint(string endpoint, bool requireAuthorization);

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <param name="globalAuthorizationPolicy">The global roles authorized for this endpoint</param>
        /// <returns></returns>
        IKwfEndpointBuilder InitializeEndpoint(string endpoint, params string[] globalAuthorizationPolicy);

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <param name="globalAuthorizationPolicy">The Global Authorization Policy from PoliciesEnum</param>
        /// <returns></returns>
        IKwfEndpointBuilder InitializeEndpoint(string endpoint, PoliciesEnum globalAuthorizationPolicy);
    }
}
