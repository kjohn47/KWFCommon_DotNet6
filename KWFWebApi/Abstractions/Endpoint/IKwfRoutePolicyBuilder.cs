namespace KWFWebApi.Abstractions.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    public interface IKwfRoutePolicyBuilder : IKwfRouteActionBuilder
    {
        /// <summary>
        /// Set Custom Policy for this specific endpoint. Will Override global settings
        /// </summary>
        /// <param name="role">The Policy from PoliciesEnum</param>
        /// <returns>IKwfRouteActionBuilder</returns>
        IKwfRouteActionBuilder SetPolicy(PoliciesEnum role);

        /// <summary>
        /// Set Custom roles for this specific endpoint. Will Override global settings
        /// </summary>
        /// <param name="roles">The roles</param>
        /// <returns>IKwfRouteActionBuilder</returns>
        IKwfRouteActionBuilder SetPolicy(params string[]? roles);

        /// <summary>
        /// Disable global authorization on this endpoint
        /// </summary>
        /// <returns>IKwfRouteActionBuilder</returns>
        IKwfRouteActionBuilder DisableGlobalRoles();
    }
}
