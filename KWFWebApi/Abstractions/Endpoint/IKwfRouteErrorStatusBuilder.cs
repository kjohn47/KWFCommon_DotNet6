namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteErrorStatusBuilder : IKwfRoutePolicyBuilder
    {
        IKwfRoutePolicyBuilder SetErrorHTTPCodes(params int[] codes);
    }
}
