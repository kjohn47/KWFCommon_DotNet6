namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteSuccessStatusBuilder : IKwfRoutePolicyBuilder
    {
        IKwfRoutePolicyBuilder SetSuccessHTTPCodes(params int[] codes);
    }
}
