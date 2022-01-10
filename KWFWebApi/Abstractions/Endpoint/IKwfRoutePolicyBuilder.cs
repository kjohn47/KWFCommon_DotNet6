namespace KWFWebApi.Abstractions.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    public interface IKwfRoutePolicyBuilder : IKwfRouteActionBuilder
    {
        IKwfRouteActionBuilder SetPolicy(PoliciesEnum role);
        IKwfRouteActionBuilder SetPolicy(params string[]? roles);
        IKwfRouteActionBuilder DisableGlobalRoles();
    }
}
